#nullable disable
using Microsoft.AspNetCore.Mvc;
using PingPong.Data;
using PingPong.Models;
using System.Data.SqlClient;

namespace PingPong.Controllers
{
    public class PlayersController : Controller
    {
        private readonly IPlayerRepository _repository;
        private readonly IGameRepository _gameRepository;

        public PlayersController(IPlayerRepository repository, IGameRepository gameRepository)
        {
            _repository = repository;
            _gameRepository = gameRepository;
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            List<Player> players = (await this._repository.FindAll()).ToList();
            return View(players);
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            const int nLastGames = 10;

            if (id == null)
            {
                return NotFound();
            }

            var player = await _repository.FindOne((int)id);
            if (player == null)
            {
                return NotFound();
            }

            var playerDetailsVM = new PlayerDetailsVM
            {
                Id = (int)id,
                Name = player.Name,
                LastGames = await _gameRepository.FindLastGamesFromPlayers((int)id, nLastGames)
            };

            return View(playerDetailsVM);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            return View("Form");
        }

        // POST: Players/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Player player)
        {
            if (ModelState.IsValid)
            {
                await _repository.Create(player);
                return RedirectToAction(nameof(Index));
            }
            return View("Form", player);
        }
        
        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _repository.FindOne((int)id);
            if (player == null)
            {
                return NotFound();
            }
            return View("Form", player);
        }

        // POST: Players/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Player player)
        {
            if (id != player.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.Update(player);
                }
                catch (SqlException)
                {
                    if (!await PlayerExists((int)player.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("Form", player);
        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var player = await _repository.FindOne((int)id);
            if (player == null)
            {
                return NotFound();
            }

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PlayerExists(int id)
        {
            return await _repository.FindOne(id) != null;
        }

    }
}
