using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VotaYa.Data;
using VotaYa.Hubs;
using VotaYa.Models;

namespace VotaYa.Controllers
{
    public class VotacionController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<VotoHub> _hub;

        public VotacionController(AppDbContext db, IHubContext<VotoHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        // ── PANEL DE ADMIN ───────────────────────────────────────────────────

        // GET: /Votacion/Admin — crear preguntas y activarlas
        public async Task<IActionResult> Admin()
        {
            var preguntas = await _db.Preguntas.Include(p => p.Opciones).ToListAsync();
            return View(preguntas);
        }

        // POST: crear una nueva pregunta con opciones
        [HttpPost]
        public async Task<IActionResult> CrearPregunta(string texto, string[] opciones)
        {
            var pregunta = new Pregunta { Texto = texto };
            foreach (var op in opciones.Where(o => !string.IsNullOrWhiteSpace(o)))
                pregunta.Opciones.Add(new Opcion { Texto = op });

            _db.Preguntas.Add(pregunta);
            await _db.SaveChangesAsync();
            return RedirectToAction("Admin");
        }

        // POST: activar una pregunta (desactiva las demás)
        [HttpPost]
        public async Task<IActionResult> Activar(int id)
        {
            var todas = await _db.Preguntas.ToListAsync();
            foreach (var p in todas) p.Activa = false;

            var pregunta = todas.FirstOrDefault(p => p.Id == id);
            if (pregunta != null) pregunta.Activa = true;

            await _db.SaveChangesAsync();
            return RedirectToAction("Admin");
        }

        // ── PANTALLA DE VOTACIÓN ─────────────────────────────────────────────

        // GET: /Votacion/Votar — lo que ve cada participante
        public async Task<IActionResult> Votar()
        {
            var pregunta = await _db.Preguntas
                .Include(p => p.Opciones)
                .FirstOrDefaultAsync(p => p.Activa);
            return View(pregunta);
        }

        // POST: registrar un voto
        [HttpPost]
        public async Task<IActionResult> RegistrarVoto(int opcionId)
        {
            var opcion = await _db.Opciones.FindAsync(opcionId);
            if (opcion == null) return BadRequest();

            opcion.Votos++;
            await _db.SaveChangesAsync();

            // SignalR notifica a todos en tiempo real
            await _hub.Clients.All.SendAsync("ResultadoActualizado", opcionId, opcion.Votos);

            return Ok();
        }

        // ── PANTALLA DE RESULTADOS ───────────────────────────────────────────

        // GET: /Votacion/Resultados — pantalla con gráfico en vivo
        public async Task<IActionResult> Resultados()
        {
            var pregunta = await _db.Preguntas
                .Include(p => p.Opciones)
                .FirstOrDefaultAsync(p => p.Activa);
            return View(pregunta);
        }
        // ... método Resultados() ...

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var pregunta = await _db.Preguntas.Include(p => p.Opciones)
                                               .FirstOrDefaultAsync(p => p.Id == id);
            if (pregunta != null)
            {
                _db.Opciones.RemoveRange(pregunta.Opciones);
                _db.Preguntas.Remove(pregunta);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Admin");
        }

    }  // ← este } cierra la clase VotacionController
}      // ← este } cierra el namespace

