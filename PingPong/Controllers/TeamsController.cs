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
        private readonly PingPongContext _context;
        private readonly PlayerRepository _playerRepository;

        public TeamsController(PingPongContext context, PlayerRepository playerRepository)
        {
            _context = context;
            _playerRepository = playerRepository;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var teamPlayerVms = new List<TeamPlayerVm>();
            foreach (var team in await _context.Team.ToListAsync())
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

            var team = await _context.Team
                .FirstOrDefaultAsync(m => m.Id == id);
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
                _context.Add(newTeam);
                await _context.SaveChangesAsync();
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

            var team = await _context.Team.FindAsync(id);
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
                    var team = await _context.Team.FindAsync(teamPlayerSelectionVM.TeamId);
                    team.Name = teamPlayerSelectionVM.TeamName;
                    team.PlayerOneId = teamPlayerSelectionVM.SelectedPlayerOneId;
                    team.PlayerTwoId = teamPlayerSelectionVM.SelectedPlayerTwoId;
                    _context.Update(team);
                    await _context.SaveChangesAsync();
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

            var team = await _context.Team
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var team = await _context.Team.FindAsync(id);
            _context.Team.Remove(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Team.Any(e => e.Id == id);
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
