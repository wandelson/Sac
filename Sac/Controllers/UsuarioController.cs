using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Sac.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace Sac.Controllers
    {
        [Authorize(Roles = "Administrador")]
        public class UsuarioController : Controller
        {
            private sacEntities1 db = new sacEntities1();

            // GET: Usuario
            public async Task<ActionResult> Index()
            {
                return View(await db.Usuario.ToListAsync());
            }

            // GET: Usuario/Details/5
            public async Task<ActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuario usuario = await db.Usuario.FindAsync(id);
                if (usuario == null)
                {
                    return HttpNotFound();
                }
                return View(usuario);
            }

            // GET: Usuario/Edit/5
            public async Task<ActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuario usuario = await db.Usuario.FindAsync(id);
                if (usuario == null)
                {
                    return HttpNotFound();
                }
                return View(usuario);
            }

            // POST: Usuario/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Edit([Bind(Include = "IdUsuario,Nome,Email,Telefone,UserName")] Usuario usuario)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        using (var ctx = new sacEntities1())
                        {
                            var usuarioOld = await ctx.Usuario.FindAsync(usuario.IdUsuario);
                            usuario.UserName = usuarioOld.UserName;
                            usuarioOld = null;
                        }

                        db.Entry(usuario).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    return View(usuario);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(usuario);
                }
            }

            // GET: Usuario/Delete/5
            public async Task<ActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Usuario usuario = await db.Usuario.FindAsync(id);
                if (usuario == null)
                {
                    return HttpNotFound();
                }
                return View(usuario);
            }

            // POST: Usuario/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> DeleteConfirmed(int id)
            {
                Usuario usuario = await db.Usuario.FindAsync(id);

                if (User.Identity.Name == usuario.UserName)
                {
                    ModelState.AddModelError("", "Você não pode excluir o seu usuário!");
                    return View(usuario);
                }

                var result = await db.Chamado.Where(c => c.IdCliente == id || c.IdAtendente == id).ToListAsync();

                if (result.Count > 0)
                {
                    ModelState.AddModelError("", "Não pode excluir usuário com chamado atrelado!");
                    return View(usuario);
                }

                db.Usuario.Remove(usuario);
                await db.SaveChangesAsync();

                var usuarioStore = new UserStore<IdentityUser>();
                var _userManager = new UserManager<IdentityUser>(usuarioStore);

                var user = await _userManager.FindByNameAsync(usuario.UserName);
                var logins = user.Logins;
                var rolesForUser = await _userManager.GetRolesAsync(User.Identity.GetUserId());

                foreach (var login in logins.ToList())
                    await _userManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        await _userManager.RemoveFromRoleAsync(user.Id, item);
                    }
                }

                await _userManager.DeleteAsync(user);



                return RedirectToAction("Index");
            }

    

}
}