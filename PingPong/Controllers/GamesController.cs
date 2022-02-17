#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PingPong.Data;
using PingPong.Models;

namespace PingPong.Controllers
{
    public class GamesController : Controller
    {
        private readonly PingPongContext _context;
        private readonly TeamRepository _teamRepository;

        public GamesController(PingPongContext context, TeamRepository teamRepository)
        {
            _context = context;
            _teamRepository = teamRepository;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            return View(await _context.Game.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public async Task<IActionResult> Create()
        {
            var teams = await _teamRepository.FindAll();
            var gameTeamSelectionVM = new GameTeamSelectionVM
            {
                Teams = new SelectList(teams, nameof(Team.Id), nameof(Team.Name))
            };
            return View(gameTeamSelectionVM);
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Date,TeamOneScore,SelectedTeamOneId,TeamTwoScore,SelectedTeamTwoId")] GameTeamSelectionVM gameTeamSelectionVM)
        {
            if (ModelState.IsValid)
            {
                var game = new Game
                {
                    Date = gameTeamSelectionVM.Date,
                    TeamOneId = gameTeamSelectionVM.SelectedTeamOneId,
                    TeamOneScore = gameTeamSelectionVM.TeamOneScore,
                    TeamTwoId = gameTeamSelectionVM.SelectedTeamTwoId,
                    TeamTwoScore = gameTeamSelectionVM.TeamTwoScore
                };
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gameTeamSelectionVM);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.FindAsync(id);
            _context.Game.Remove(game);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.Id == id);
        }
    }
}
