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
    public class ChamadoController : Controller
    {

        private sacEntities db = new sacEntities();


        public async Task<ActionResult> Index(string estado)
        {

            BindUrgencia();
            BindEstado();

            IQueryable<Chamado> chamado;

            if (User.IsInRole("Administrador") || User.IsInRole("Atendente"))
            {
                chamado = db.Chamado.Include(c => c.Usuario)
                                        .Include(c => c.Usuario1);
            }
            else
            {
                chamado = db.Chamado.Include(c => c.Usuario)
                                         .Include(c => c.Usuario1)
                                         .Where(c => c.Usuario1.UserName == User.Identity.Name);
            }


            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                chamado = db.Chamado.Where(x => x.Estado == estado);
            }


            return View(await chamado.ToListAsync());
        }

        // GET: Chamado/Details/5

        [Authorize(Roles = "Atendente")]
        public async Task<ActionResult> Solucionar(int? id)
        {
            BindUrgencia();
            BindEstado();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chamado chamado = await db.Chamado.FindAsync(id);
            if (chamado == null)
            {
                return HttpNotFound();
            }
            return View(chamado);
        }
        [Authorize(Roles = "Atendente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Solucionar(int? id, string Solucao)
        {

            BindUrgencia();
            BindEstado();

            var chamado = await db.Chamado.FindAsync(id);
            chamado.Solucao = Solucao;

                var usuario = await db.Usuario.Where(u => u.UserName == User.Identity.Name).FirstAsync();
                chamado.IdAtendente = usuario.IdUsuario;

            chamado.Estado = "Finalizado";

            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(chamado.Solucao))
            {
                db.Entry(chamado).State = EntityState.Modified;
                await db.SaveChangesAsync();
                BindUrgencia();
                BindEstado();
                return RedirectToAction("Index");
            }

           

            ModelState.AddModelError("", "Solução é obrigatória!");
            ViewBag.IdAtendente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdAtendente);
            ViewBag.IdCliente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdCliente);

            return View(chamado);
        }
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chamado chamado = await db.Chamado.FindAsync(id);
            if (chamado == null)
            {
                return HttpNotFound();
            }

            BindUrgencia();
            BindEstado();
            return View(chamado);
        }


        private void BindUrgencia()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Simples", Value = "Simples" });

            items.Add(new SelectListItem { Text = "Moderada", Value = "Moderado" });            

            items.Add(new SelectListItem { Text = "Grave", Value = "Grave" });

            ViewBag.Urgencia = items;
        }



        private void BindEstado()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Pendente", Value = "Pendente" });

            items.Add(new SelectListItem { Text = "Aguardando", Value = "Aguardando" });

            items.Add(new SelectListItem { Text = "Finalizado", Value = "Finalizado" });

            ViewBag.Estado = items;
        }


        // GET: Chamado/Create
        public ActionResult Create()
        {
      
            BindUrgencia();
            BindEstado();
            ViewBag.IdAtendente = new SelectList(db.Usuario, "IdUsuario", "Nome");
            ViewBag.IdCliente = new SelectList(db.Usuario, "IdUsuario", "Nome");
            return View();
        }

        // POST: Chamado/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( Chamado chamado)
        {
            chamado.DataAbertura = DateTime.Now;
            chamado.Estado = "Aguardando";

            var usuario = await db.Usuario.Where(u => u.UserName == User.Identity.Name).FirstAsync();
            chamado.IdCliente = usuario.IdUsuario;
        

            if (ModelState.IsValid)
            {

       
                db.Chamado.Add(chamado);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IdAtendente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdAtendente);
            ViewBag.IdCliente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdCliente);
            return View(chamado);
        }

        // GET: Chamado/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chamado chamado = await db.Chamado.FindAsync(id);
            if (chamado == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAtendente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdAtendente);
            ViewBag.IdCliente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdCliente);
            BindUrgencia();
            BindEstado();
            return View(chamado);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int id ,Chamado chamado)
        {

            var domain = db.Chamado.Where(x => x.IdChamado == id).SingleOrDefault();

            domain.Estado = chamado.Estado;
            domain.Descricao = chamado.Descricao;
            domain.Solucao = chamado.Solucao;
            domain.Urgencia = chamado.Urgencia;


            if (ModelState.IsValid)
            {
                db.Entry(domain).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdAtendente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdAtendente);
            ViewBag.IdCliente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdCliente);
            BindUrgencia();
            BindEstado();
            return View(chamado);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult> FecharChamado(int? id)
        {
            var chamado = await db.Chamado.FindAsync(id);
            chamado.Estado = "Finalizado";

            db.Entry(chamado).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}