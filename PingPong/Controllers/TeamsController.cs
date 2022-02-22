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

        public TeamsController(IPlayerRepository playerRepository, ITeamRepository teamRepository)
        {
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
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

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(await ToTeamPlayerVm(team));
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
                    PlayerOneId = teamPlayerSelectionVM.SelectedPlayerOneId,
                    PlayerTwoId = teamPlayerSelectionVM.SelectedPlayerTwoId
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
                NPlayers = team.PlayerTwoId == null ? 1u : 2u,
                SelectedPlayerOneId = team.PlayerOneId,
                SelectedPlayerTwoId = team.PlayerTwoId
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
                    team.PlayerOneId = teamPlayerSelectionVM.SelectedPlayerOneId;
                    team.PlayerTwoId = teamPlayerSelectionVM.SelectedPlayerTwoId;
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
            var playerOne = await _playerRepository.FindOne(team.PlayerOneId);
            Player playerTwo = null;
            if (team.PlayerTwoId != null)
            {
                playerTwo = await _playerRepository.FindOne((int)team.PlayerTwoId);
            } 
            return new TeamPlayerVm
            {
                Id = team.Id,
                Name = team.Name,
                PlayerOne = playerOne,
                PlayerTwo = playerTwo
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
