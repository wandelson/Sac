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


        public async Task<ActionResult> Index(int? IdEstado)
        {
            IQueryable<Chamado> chamado;

            if (User.IsInRole("Administrador") || User.IsInRole("Atendente"))
            {
                chamado = db.Chamado.Include(c => c.Estado)
                                        .Include(c => c.Urgencia)
                                        .Include(c => c.Usuario)
                                        .Include(c => c.Usuario1);
            }
            else
            {
                chamado = db.Chamado.Include(c => c.Estado)
                                         .Include(c => c.Urgencia)
                                         .Include(c => c.Usuario)
                                         .Include(c => c.Usuario1)
                                         .Where(c => c.Usuario1.UserName == User.Identity.Name);
            }


            return View(await chamado.ToListAsync());
        }

        // GET: Chamado/Details/5

        [Authorize(Roles = "Atendente")]
        public async Task<ActionResult> Solucionar(int? id)
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
            return View(chamado);
        }
        [Authorize(Roles = "Atendente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Solucionar(int? id, string Solucao)
        {
            var chamado = await db.Chamado.FindAsync(id);
            chamado.Solucao = Solucao;

                var usuario = await db.Usuario.Where(u => u.UserName == User.Identity.Name).FirstAsync();
                chamado.IdAtendente = usuario.IdUsuario;

            chamado.Estado = "Pendente";

            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(chamado.Solucao))
            {
                db.Entry(chamado).State = EntityState.Modified;
                await db.SaveChangesAsync();
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
            return View(chamado);
        }


        private void BindUrgencia()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Action", Value = "0" });

            items.Add(new SelectListItem { Text = "Drama", Value = "1" });            

            items.Add(new SelectListItem { Text = "Science Fiction", Value = "3" });

            ViewBag.Urgencia = items;
        }


        // GET: Chamado/Create
        public ActionResult Create()
        {
            BindUrgencia();

            ViewBag.IdAtendente = new SelectList(db.Usuario, "IdUsuario", "Nome");
            ViewBag.IdCliente = new SelectList(db.Usuario, "IdUsuario", "Nome");
            return View();
        }

        // POST: Chamado/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IdChamado,DataAbertura,Descricao,IdCliente,IdEstado,IdUrgencia,IdAtendente,Solucao,DataEncerramento")] Chamado chamado)
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
            return View(chamado);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> Edit([Bind(Include = "IdChamado,DataAbertura,Descricao,IdCliente,IdEstado,IdUrgencia,IdAtendente,Solucao,DataEncerramento")] Chamado chamado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chamado).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IdAtendente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdAtendente);
            ViewBag.IdCliente = new SelectList(db.Usuario, "IdUsuario", "Nome", chamado.IdCliente);
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