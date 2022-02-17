#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PingPong.Data;
using PingPong.Models;

namespace PingPong.Controllers
{
    public class TeamsController : Controller
    {
        private readonly PlayerRepository _playerRepository;
        private readonly TeamRepository _teamRepository;

        public TeamsController(PlayerRepository playerRepository, TeamRepository teamRepository)
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
            var players = await _playerRepository.FindAll();
            var playersVm = new TeamPlayerSelectionVM
            {
                Players = new SelectList(players, nameof(Player.Id), nameof(Player.Name))
            };
            return View(playersVm);
        }

        // POST: Teams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamName,SelectedPlayerOneId,SelectedPlayerTwoId")] TeamPlayerSelectionVM teamPlayerSelectionVM)
        {
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
            return RedirectToAction(nameof(Create));
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

            var players = await _playerRepository.FindAll();
            var playersVm = new TeamPlayerSelectionVM
            {
                TeamId = team.Id,
                TeamName = team.Name,
                Players = new SelectList(players, nameof(Player.Id), nameof(Player.Name)),
                SelectedPlayerOneId = team.PlayerOneId
            };
            return View(playersVm);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("TeamId, TeamName,SelectedPlayerOneId,SelectedPlayerTwoId")] TeamPlayerSelectionVM teamPlayerSelectionVM)
        {
            if (id != teamPlayerSelectionVM.TeamId)
            {
                return NotFound();
            }

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
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists((int)teamPlayerSelectionVM.TeamId))
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
            return RedirectToAction(nameof(Edit), teamPlayerSelectionVM.TeamId);
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

        private bool TeamExists(int id)
        {
            return _teamRepository.FindOne(id) != null;
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
    }
}
