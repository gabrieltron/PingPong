#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PingPong.Data;
using PingPong.Models;
using System.Data.SqlClient;

namespace PingPong.Controllers
{
    public class TeamsController : Controller
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IGameRepository _gameRepository;

        public TeamsController(IPlayerRepository playerRepository, ITeamRepository teamRepository, IGameRepository gameRepository)
        {
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _gameRepository = gameRepository;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var teamPlayerVms = new List<TeamPlayerVm>();
            foreach (var team in await _teamRepository.FindAll())
            {
                var teamPlayerVm = await ToTeamPlayerVm(team);
                teamPlayerVms.Add(teamPlayerVm);
            }
            return View(teamPlayerVms);
        }

        public async Task<IActionResult> Leaderboard()
        {
            const int singleLeaderboardSize = 3;
            const int doubleLeaderboardSize = 3;
            var leaderboardVM = new TeamLeaderboardVM
            {
                SingleTeamLeaderboards = await _teamRepository.FindSingleTeamLeaderboard(singleLeaderboardSize),
                DoubleTeamLeaderboards = await _teamRepository.FindDoubleTeamLeaderboard(doubleLeaderboardSize)
            };
            return View(leaderboardVM);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            const int nLastGames = 10;

            if (id == null)
            {
                return NotFound();
            }

            var team = await _teamRepository.FindOne((int)id);
            if (team == null)
            {
                return NotFound();
            }

            var teamDetailsVM = new TeamDetailsVM
            {
                Id = (int)id,
                Name = team.Name,
                PlayerOne = team.PlayerOne,
                PlayerTwo = team.PlayerTwo,
                LastGames = await _gameRepository.FindLastGamesFromTeam((int)id, nLastGames)
            };
            return View(teamDetailsVM);
        }

        // GET: Teams/Create
        public async Task<IActionResult> Create()
        {
            var playersVm = new TeamPlayerSelectionVM
            {
                Players = await _playerRepository.FindAll()
        };
            return View("Form", playersVm);
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamPlayerSelectionVM teamPlayerSelectionVM)
        {
            await CheckTeamIsUnique(teamPlayerSelectionVM);
            if (ModelState.IsValid)
            {
                var newTeam = new Team
                {
                    Name = teamPlayerSelectionVM.TeamName,
                    PlayerOne = new Player { Id = teamPlayerSelectionVM.SelectedPlayerOneId },
                    PlayerTwo = new Player { Id = teamPlayerSelectionVM.SelectedPlayerTwoId }
                };
                await _teamRepository.Create(newTeam);
                return RedirectToAction(nameof(Index));
            }
            var playersVm = new TeamPlayerSelectionVM
            {
                TeamId = teamPlayerSelectionVM.TeamId,
                TeamName = teamPlayerSelectionVM.TeamName,
                Players = await _playerRepository.FindAll(),
                NPlayers = teamPlayerSelectionVM.NPlayers,
                SelectedPlayerOneId = teamPlayerSelectionVM.SelectedPlayerOneId,
                SelectedPlayerTwoId = teamPlayerSelectionVM.SelectedPlayerTwoId
            };
            return View("Form", playersVm);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _teamRepository.FindOne((int)id);
            if (team == null)
            {
                return NotFound();
            }

            var playersVm = new TeamPlayerSelectionVM
            {
                TeamId = team.Id,
                TeamName = team.Name,
                Players = await _playerRepository.FindAll(),
                NPlayers = team.PlayerTwo == null ? 1u : 2u,
                SelectedPlayerOneId = (int)team.PlayerOne.Id,
                SelectedPlayerTwoId = team.PlayerTwo?.Id
            };
            return View("Form", playersVm);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeamPlayerSelectionVM teamPlayerSelectionVM)
        {
            if (id != teamPlayerSelectionVM.TeamId)
            {
                return NotFound();
            }

            await CheckTeamIsUnique(teamPlayerSelectionVM);
            if (ModelState.IsValid)
            {
                try
                {
                    var team = await _teamRepository.FindOne((int)teamPlayerSelectionVM.TeamId);
                    team.Name = teamPlayerSelectionVM.TeamName;
                    team.PlayerOne = new Player { Id = teamPlayerSelectionVM.SelectedPlayerOneId };
                    team.PlayerTwo = new Player { Id = teamPlayerSelectionVM.SelectedPlayerTwoId };
                    await _teamRepository.Update(team);
                }
                catch (SqlException)
                {
                    if (!await TeamExists((int)teamPlayerSelectionVM.TeamId))
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
            var newTeamPlayerSelectionVM = new TeamPlayerSelectionVM
            {
                TeamId = teamPlayerSelectionVM.TeamId,
                TeamName = teamPlayerSelectionVM.TeamName,
                Players = await _playerRepository.FindAll(),
                NPlayers = teamPlayerSelectionVM.NPlayers,
                SelectedPlayerOneId = teamPlayerSelectionVM.SelectedPlayerOneId,
                SelectedPlayerTwoId = teamPlayerSelectionVM.SelectedPlayerTwoId
            };
            return View("Form", newTeamPlayerSelectionVM);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _teamRepository.FindOne((int)id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TeamExists(int id)
        {
            return await _teamRepository.FindOne(id) != null;
        }

        private async Task<TeamPlayerVm> ToTeamPlayerVm(Team team)
        {
            return new TeamPlayerVm
            {
                Id = team.Id,
                Name = team.Name,
                PlayerOne = team.PlayerOne,
                PlayerTwo = team.PlayerTwo
            };
        }

        private async Task CheckTeamIsUnique(TeamPlayerSelectionVM teamPlayerSelectionVM)
        {
            var firstPlayerId = teamPlayerSelectionVM.SelectedPlayerOneId;
            var secondPlayerId = teamPlayerSelectionVM.SelectedPlayerTwoId;
            var existingTeam = await _teamRepository.FindByPlayers(firstPlayerId, secondPlayerId);
            if (existingTeam != null)
            {
                const string errorMessage = "A team with the same players already exists";
                ModelState.AddModelError(String.Empty, errorMessage);
            }
        }
    }
}
