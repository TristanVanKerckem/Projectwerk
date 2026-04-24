using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using ProjectbeheerBL.Exeptions;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ProjectbeheerBL.Beheerder
{
    public class ProjectManager
    {
        private readonly IProjectRepository _repo;
        private readonly IPDFschrijver _pdfSchrijver;
        private readonly ICSVschrijver _csvSchrijver;
        private readonly IAdminRepository _adminRepo;

        public ProjectManager(IProjectRepository repo, IPDFschrijver pdfSchrijver, ICSVschrijver csvSchrijver, IAdminRepository adminrepo)
        {
            _repo = repo;
            _pdfSchrijver = pdfSchrijver;
            _csvSchrijver = csvSchrijver;
            _adminRepo = adminrepo;
        }

        /// <summary>

        /// </summary>
        // verder aanvullen voor controle op connectiestring & transactie
        public void VoegProjectToe(Project project, Partner? partner, Locatie? locPartner, List<string> rollen, IDbConnection conn, IDbTransaction trans)
        {
            if (project == null)
            {
                throw new ProjectException("Project mag niet leeg zijn.");
            }

            
            if (project is Stadsontwikkeling stads)
            {
                foreach (var firma in stads.Bouwfirmas)
                {
                    if (string.IsNullOrEmpty(firma.Naam))
                    {
                        throw new BouwfirmaException("Bouwfirma moet een naam hebben.");
                    }
                }
            }
            _repo.VoegProjectToe(project, partner, locPartner, rollen, conn, trans);
        }


        public void MaakProjectenAan(List<Project> projecten, Gebruiker gebruiker, Partner? partner, Locatie? locPartner, List<string> rollen) {

            if (projecten == null || projecten.Count == 0)
                throw new ProjectException("Er moet minstens één project worden meegegeven.");

            if ((rollen == null || rollen.Count == 0) && partner != null)
                throw new ProjectPartnerException("Er moet minstens één rol worden opgegeven per partner.");

            foreach (Project p in projecten) {
                if (p is Stadsontwikkeling stads && (stads.Bouwfirmas == null || stads.Bouwfirmas.Count == 0)) {
                    throw new StadsontwikkelingException($"Project {p.Titel} is een stadsontwikkeling en moet minstens één bouwfirma hebben.");
                }
            }

            try {
                _adminRepo.gebruikerVoegtProjectToe(projecten, gebruiker, partner, locPartner, rollen);
            } catch (ProjectException) { // zorgt dat de originele fout wordt opgeslagen en verder gegaan, zorgt ook dat de originele "stack trace" wordt behouden
                throw;
            } catch (Exception ex) {
                throw new ProjectException("Er is een fout opgetreden bij het opslaan in de database.", ex);
            }
        }

        public void PasProjectAan(ProjectCombinatie projecten, Partner? partner, Locatie? locPartner, List<string> rollen) {
            if ((projecten.ProjectComboLijst == null || projecten.ProjectComboLijst.Count < 1))
                throw new ProjectException("Er moet minstens één project al bestaan.");

            if ((rollen == null || rollen.Count == 0) && partner != null)
                throw new ProjectPartnerException("Er moet minstens één rol worden opgegeven per partner.");

            foreach (Project p in projecten.ProjectComboLijst) {
                if (p is Stadsontwikkeling stads && (stads.Bouwfirmas == null || stads.Bouwfirmas.Count == 0)) {
                    throw new StadsontwikkelingException($"Er moet minstens één bouwfirma zijn per stadsontwikkelingsproject.");
                }
            }

            try {
                _adminRepo.UpdateInformatieProject(projecten.ProjectComboLijst.First(),projecten); // Projectinfo binnen onze combo klassen is voor alle projecten hierin hetzelfde, dus nemen we de eerste
            } catch (ProjectException) { // zorgt dat de originele fout wordt opgeslagen en verder gegaan, zorgt ook dat de originele "stack trace" wordt behouden
                throw;
            } catch (Exception ex) {
                throw new ProjectException("Er is een fout opgetreden bij het opslaan van de nieuwe data in de database.", ex);
            }
        }

        public void VerwijderProject(int projectId) {

            try {
                _adminRepo.VerwijderProject(projectId);
            } catch (ProjectException) { // zorgt dat de originele fout wordt opgeslagen en verder gegaan, zorgt ook dat de originele "stack trace" wordt behouden
                throw;
            } catch (Exception ex) {
                throw new ProjectException("Er is een fout opgetreden bij het opslaan van de nieuwe data in de database.", ex);
            }
        }


        public List<Project> FilterProjecten(string? type, string? wijk, ProjectStatus? status, DateTime? start, DateTime? eind, string? partner)
        {
            try
            {
                
                return _repo.GeefProjectenMetFilters(type, wijk, status, start, eind, partner);
            }
            catch (Exception ex)
            {
                throw new ProjectException("Fout bij het filteren: " + ex.Message);
            }
        }

        public void ExporteerProjectenNaarCSV(string pad)
        {
            try
            {
                List<ProjectCombinatie> projecten = _repo.GeefAlleProjecten(); 
                if (projecten == null || projecten.Count == 0)
                {
                    throw new ProjectException("Er zijn geen projecten om te exporteren."); 
                }
                _csvSchrijver.MaakCSV(projecten, pad); 
            }
            catch (Exception ex)
            {
                throw new ProjectException("Fout bij het exporteren naar CSV: " + ex.Message);
            }
        }


        //public void MaakProjectFiche(int id, string pad)
        //{
        //    var project = _repo.GeefProject(id); 

        //    if (project == null)
        //    {
        //        throw new ProjectException("Project niet gevonden.");
        //    }

        //    try
        //    {
                
               
        //        var projectLijst = new List<Project> { project };
                
        //        byte[] pdfData = _pdfSchrijver.MaakPDF(projectLijst);

        //        System.IO.File.WriteAllBytes(pad, pdfData);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ProjectException("Fout bij het maken van de projectfiche: " + ex.Message);
        //    }
        //}
        public void MaakProjectFiche(List<ProjectCombinatie> projecten, string pad)
        {

            if (projecten == null || projecten.Count == 0) return;

            _csvSchrijver.MaakCSV(projecten, pad);
        }


        public List<ProjectCombinatie> GeefAlleProjecten()
        {
            return _repo.GeefAlleProjecten();
        }
    }
    
}
