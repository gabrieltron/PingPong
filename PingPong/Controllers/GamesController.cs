#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PingPong.Data;
using PingPong.Models;

namespace PingPong.Controllers
{
    public class GamesController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IGameRepository _gameRepository;

        public GamesController(ITeamRepository teamRepository, IGameRepository gameRepository)
        {
            _teamRepository = teamRepository;
            _gameRepository = gameRepository;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            var gameTeamVMs = new List<GameTeamVM>();
            foreach (var game in await _gameRepository.FindAll())
            {
                var gameTeamVM = await ToGameTeamVM(game);
                gameTeamVMs.Add(gameTeamVM);
            }

            return View(gameTeamVMs);
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _gameRepository.FindOne((int)id);
            if (game == null)
            {
                return NotFound();
            }

            return View(await ToGameTeamVM(game));
        }

        // GET: Games/Create
        public async Task<IActionResult> Create()
        {
            var gameTeamSelectionVM = new GameTeamSelectionVM
            {
                SingleTeams = await _teamRepository.FindSingleTeams(),
                DoubleTeams = await _teamRepository.FindDoubleTeams()
            };
            return View(gameTeamSelectionVM);
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameTeamSelectionVM gameTeamSelectionVM)
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
                await _gameRepository.Create(game);
                return RedirectToAction(nameof(Index));
            }

            var singleTeams = await _teamRepository.FindSingleTeams();
            var doubleTeams = await _teamRepository.FindDoubleTeams();
            var newGameTeamSelectionVM = new GameTeamSelectionVM
            {
                TeamsSize = gameTeamSelectionVM.TeamsSize,
                SingleTeams = await _teamRepository.FindSingleTeams(),
                SelectedTeamOneId = gameTeamSelectionVM.SelectedTeamOneId,
                DoubleTeams = await _teamRepository.FindDoubleTeams(),
                SelectedTeamTwoId = gameTeamSelectionVM.SelectedTeamTwoId
            };
            return View(newGameTeamSelectionVM);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _gameRepository.FindOne((int)id);
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
            await _gameRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<GameTeamVM> ToGameTeamVM(Game game)
        {
            var teamOne = await _teamRepository.FindOne(game.TeamOneId);
            var teamTwo = await _teamRepository.FindOne(game.TeamTwoId);
            return new GameTeamVM
            {
                Id = game.Id,
                Date = game.Date,
                TeamOne = teamOne,
                TeamOneScore = game.TeamOneScore,
                TeamTwo = teamTwo,
                TeamTwoScore = game.TeamTwoScore
            };
        }

    }
}
