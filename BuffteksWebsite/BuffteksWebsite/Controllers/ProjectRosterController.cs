using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BuffteksWebsite.Models;

namespace BuffteksWebsite.Controllers
{
    public class ProjectRosterController : Controller
    {
        private readonly BuffteksWebsiteContext _context;

        public ProjectRosterController(BuffteksWebsiteContext context)
        {
            _context = context;
        }

        // GET: ProjectRoster
        public async Task<IActionResult> Index()
        {
            List<ProjectRoster> rosters = await _context.ProjectRoster.Include(p => p.Project).Include(p => p.ProjectParticipant).ToListAsync();
            foreach (ProjectRoster roster in rosters) {
                roster.ProjectParticipant.Type = MemberExists(roster.ProjectParticipantID) ? "Member" : "Client";
            }
            return View(rosters);
        }
        
        // GET: ProjectRoster/Delete/5
        public async Task<IActionResult> RemoveFromRoster(string ProjectID, string ProjectParticipantID)
        {
            if (ProjectID == null || ProjectParticipantID == null)
            {
                return NotFound();
            }

            var projectRoster = await _context.ProjectRoster
                .Include(p => p.Project)
                .Include(p => p.ProjectParticipant)
                .SingleOrDefaultAsync(m => m.ProjectID == ProjectID && m.ProjectParticipantID == ProjectParticipantID);
            if (projectRoster == null)
            {
                return NotFound();
            }

            return View(projectRoster);
        }

        // POST: ProjectRoster/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromRosterConfirmed(string ProjectID, string ProjectParticipantID)
        {
            var projectRoster = await _context.ProjectRoster.SingleOrDefaultAsync(m => m.ProjectID == ProjectID && m.ProjectParticipantID == ProjectParticipantID);
            _context.ProjectRoster.Remove(projectRoster);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectRosterExists(string ProjectID, string ProjectParticipantID)
        {
            return _context.ProjectRoster.Any(e => e.ProjectID == ProjectID && e.ProjectParticipantID == ProjectParticipantID);
        }

        private bool MemberExists(string memberID) {
            return _context.Members.Any(e => e.ID == memberID);
        }
    }
}
