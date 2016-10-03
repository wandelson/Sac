using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Sac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Sac.Controllers
{
    public class AutenticacaoController : Controller
    {
        // GET: Autenticacao
        private sacEntities1  db = new sacEntities1();


        private void CarregaRoles()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
            var lista = roleManager.Roles.ToList();

            ViewBag.Roles = new SelectList(lista, "Id", "Name");
        }

        // GET: Autenticacao
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        // [Authorize(Roles = "Administrador")]
        public ActionResult Criar()
        {
            CarregaRoles();
            return View();
        }


        [HttpPost]
        // [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Criar(UsuarioViewModel usuario)
        {
            if (!ModelState.IsValid)
            {
                CarregaRoles();
                return View(usuario);
            }
            else
            {
                var usuarioStore = new UserStore<IdentityUser>();
                var usuarioManager = new UserManager<IdentityUser>(usuarioStore);
                //Cria uma identidade
                var usuarioInfo = new IdentityUser()
                {
                    UserName = usuario.Username
                };

                //Cria o usuário
                IdentityResult resultado = usuarioManager.Create(usuarioInfo, usuario.Senha);
                //se o usuário foi criado, o autentica
                if (resultado.Succeeded)
                {

                    var identityUserRole = new IdentityUserRole();
                    identityUserRole.RoleId = usuario.Papel;
                    identityUserRole.UserId = usuarioInfo.Id;

                    usuarioInfo.Roles.Add(identityUserRole);

                    usuarioManager.Update(usuarioInfo);

                    var usuarioDb = new Usuario()
                    {
                        Nome = usuario.Nome,
                        Telefone = "",
                        Email = usuario.Email,
                        UserName = usuario.Username
                    };

                    db.Usuario.Add(usuarioDb);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", "Usuario");
                }
                else
                {
                    CarregaRoles();
                    ModelState.AddModelError("", resultado.Errors.FirstOrDefault());
                    return View(usuario);
                }
            }
        }


        [HttpPost]
        public ActionResult Login(UsuarioViewModel usuario, string ReturnUrl)
        {
            if (string.IsNullOrWhiteSpace(usuario.Username) || string.IsNullOrWhiteSpace(usuario.Senha))
            {
                ModelState.AddModelError("", "Entre com o usuário e senha!");
                return View(usuario);
            }
            else
            {
                var usuarioStore = new UserStore<IdentityUser>();
                var usuarioManager = new UserManager<IdentityUser>(usuarioStore);
                var usuarioInfo = usuarioManager
                .Find(usuario.Username, usuario.Senha);
                if (usuarioInfo != null)
                {
                    var autManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                    var identidadeUsuario = usuarioManager
                    .CreateIdentity(usuarioInfo,
                    DefaultAuthenticationTypes.ApplicationCookie);
                    autManager.SignIn(new AuthenticationProperties()
                    { IsPersistent = false }, identidadeUsuario);

                    if (ReturnUrl == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Redirect(ReturnUrl);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Usuário/senha inválido!");
                    return View();
                }
            }
        }

        public ActionResult Logout()
        {
            var autManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
            autManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}