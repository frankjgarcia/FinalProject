using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BuffteksWebsite.Models;

namespace BuffteksWebsite.Controllers {
    public class ProjectsController : Controller {
        private readonly BuffteksWebsiteContext _context;

        public ProjectsController(BuffteksWebsiteContext context) {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index() {
            return View(await _context.Projects.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(string id) {
            if (id == null) {
                return NotFound();
            }

            var project = await _context.Projects
                .SingleOrDefaultAsync(m => m.ID == id);

            if (project == null) {
                return NotFound();
            }

            var clients =
                from participant in _context.Clients
                join projectparticipant in _context.ProjectRoster
                on participant.ID equals projectparticipant.ProjectParticipantID
                where project.ID == projectparticipant.ProjectID
                select participant;

            var members =
                from participant in _context.Members
                join projectparticipant in _context.ProjectRoster
                on participant.ID equals projectparticipant.ProjectParticipantID
                where project.ID == projectparticipant.ProjectID
                select participant;

            ProjectDetailViewModel pdvm = new ProjectDetailViewModel {
                TheProject = project,
                ProjectClients = clients.ToList() ?? null,
                ProjectMembers = members.ToList() ?? null
            };

            return View(pdvm);
        }

        // GET: Projects/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProjectName,ProjectDescription")] Project project) {
            if (ModelState.IsValid) {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(string id) {
            if (id == null) {
                return NotFound();
            }

            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ID == id);
            if (project == null) {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,ProjectName,ProjectDescription")] Project project) {
            if (id != project.ID) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!ProjectExists(project.ID)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/EditProjectParticipants/5
        public async Task<IActionResult> EditProjectParticipants(string id) {
            if (id == null) {
                return NotFound();
            }

            //var project = await _context.Projects.SingleOrDefaultAsync(m => m.ID == id);
            //if (project == null) {
            //    return NotFound();
            //}

            ////var clients = await _context.Clients.ToListAsync();

            ////CLIENTS
            ////pull 'em into lists first
            //var clients = await _context.Clients.ToListAsync();
            //var projectroster = await _context.ProjectRoster.ToListAsync();

            ///*
            //var uniqueclients = 
            //    from participant in clients
            //    join projectparticipant in projectroster
            //    on participant.ID equals projectparticipant.ProjectParticipantID
            //    where participant.ID != projectparticipant.ProjectParticipantID
            //    select participant;
            //*/

            //List<SelectListItem> clientsSelectList = new List<SelectListItem>();

            //foreach (var client in clients) {
            //    clientsSelectList.Add(new SelectListItem { Value = client.ID, Text = client.FirstName + " " + client.LastName });
            //}

            ////MEMBERS
            ////pull 'em into lists first
            //var members = await _context.Members.ToListAsync();

            ///*
            //var uniquemembers = 
            //    from participant in members
            //    join projectparticipant in projectroster
            //    on participant.ID equals projectparticipant.ProjectParticipantID
            //    where participant.ID != projectparticipant.ProjectParticipantID
            //    select participant;    
            //*/

            //List<SelectListItem> membersSelectList = new List<SelectListItem>();

            //foreach (var member in members) {
            //    membersSelectList.Add(new SelectListItem { Value = member.ID, Text = member.FirstName + " " + member.LastName });
            //}

            //create and prepare ViewModel
            //EditProjectDetailViewModel epdvm = new EditProjectDetailViewModel {
            //    TheProject = project,
            //    ProjectClientsList = clientsSelectList,
            //    ProjectMembersList = membersSelectList
            //};

            var client =
                await (from participant in _context.Clients
                       join projectparticipant in _context.ProjectRoster
                       on participant.ID equals projectparticipant.ProjectParticipantID
                       where projectparticipant.ProjectID == id
                       select participant).FirstOrDefaultAsync();

            EditProjectDetailViewModel epdvm = await getPopulatedEditProjectDetailViewModel(id, client?.ID);
            if (epdvm.TheProject == null) {
                return NotFound();
            }
            return View(epdvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProjectParticipants(string ProjectId, string SelectedID, string ParticipantType) {
            if (string.IsNullOrWhiteSpace(ProjectId)) {
                return NotFound();
            } else if (string.IsNullOrWhiteSpace(SelectedID)) {
                return View(await getPopulatedEditProjectDetailViewModel(ProjectId, null, "Error: Project Participant not selected."));
            }

            if (ModelState.IsValid) {
                try {
                    if (ParticipantType == "Client" && !ProjectRosterExists(ProjectId, SelectedID)) {
                        List<Client> clients = await _context.Clients.Where(c => c.Projects.Where(p => p.ProjectID == ProjectId).Any()).ToListAsync();
                        foreach (Client client in clients) {
                            if (client != null) {
                                ProjectRoster projectRoster = await _context.ProjectRoster.Where(pr => pr.ProjectID == ProjectId && pr.ProjectParticipantID == client.ID).FirstOrDefaultAsync();
                                if (projectRoster != null) {
                                    _context.Remove(projectRoster);
                                }
                            }
                        }
                    }

                    if (!ProjectRosterExists(ProjectId, SelectedID) || ParticipantType == "Member") {
                        ProjectRoster roster = new ProjectRoster();
                        roster.ProjectID = ProjectId;
                        roster.ProjectParticipantID = SelectedID;
                        _context.Add(roster);
                        await _context.SaveChangesAsync();
                    }
                } catch (Exception) {
                    if (!ProjectExists(ProjectId) || !ProjectParticipantExists(SelectedID)) {
                        return NotFound();
                    } else if (ProjectRosterExists(ProjectId, SelectedID)) {
                        return View(await getPopulatedEditProjectDetailViewModel(ProjectId, SelectedID, "Error: Selected Participant has already been added to this project."));
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(await getPopulatedEditProjectDetailViewModel(ProjectId, SelectedID, "Error: An error occured while saving. Please reload the page and try again."));
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(string id) {
            if (id == null) {
                return NotFound();
            }

            var project = await _context.Projects
                .SingleOrDefaultAsync(m => m.ID == id);
            if (project == null) {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id) {
            var project = await _context.Projects.SingleOrDefaultAsync(m => m.ID == id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(string id) {
            return _context.Projects.Any(e => e.ID == id);
        }

        private bool ProjectParticipantExists(string participantID) {
            return _context.Members.Any(e => e.ID == participantID) || _context.Clients.Any(e => e.ID == participantID);
        }

        private bool ProjectRosterExists(string projectId, string projectParticipantID) {
            return _context.ProjectRoster.Any(e => e.ProjectID == projectId && e.ProjectParticipantID == projectParticipantID);
        }

        public async Task<EditProjectDetailViewModel> getPopulatedEditProjectDetailViewModel(string projectId, string selectedId = null, string errorMessage = "") {
            var clients = await _context.Clients.Where(c => c.Projects.Count == 0 || c.Projects.Where(p => p.ProjectID == projectId).Any()).ToListAsync();
            List<SelectListItem> clientsSelectList = new List<SelectListItem>();
            clientsSelectList.Add(new SelectListItem { Value = "", Text = "Select a Client" });
            foreach (var client in clients) {
                clientsSelectList.Add(new SelectListItem { Value = client.ID, Text = client.FirstName + " " + client.LastName });
            }

            var members = await _context.Members.Where(c => c.Projects.Count == 0).ToListAsync();
            List<SelectListItem> membersSelectList = new List<SelectListItem>();
            membersSelectList.Add(new SelectListItem { Value = "", Text = "Select a Member" });
            foreach (var member in members) {
                membersSelectList.Add(new SelectListItem { Value = member.ID, Text = member.FirstName + " " + member.LastName });
            }

            return new EditProjectDetailViewModel {
                TheProject = await _context.Projects.SingleOrDefaultAsync(m => m.ID == projectId),
                ProjectClientsList = clientsSelectList,
                ProjectMembersList = membersSelectList,
                SelectedID = selectedId,
                ErrorMessage = (!string.IsNullOrWhiteSpace(errorMessage)) ? "<div style='color: red; font-weight:bold; margin-bottom:12px;'>" + errorMessage + "</div>" : ""
            };
        }
    }
}
