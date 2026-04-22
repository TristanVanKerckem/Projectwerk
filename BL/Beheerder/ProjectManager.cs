using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using ProjectbeheerBL.Exeptions;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectbeheerBL.Beheerder
{
    public class ProjectManager
    {
        private readonly IProjectRepository _repo;
        private readonly IPDFschrijver _pdfSchrijver;
        private readonly ICSVschrijver _csvSchrijver;

        public ProjectManager(IProjectRepository repo, IPDFschrijver pdfSchrijver, ICSVschrijver csvSchrijver)
        {
            _repo = repo;
            _pdfSchrijver = pdfSchrijver;
            _csvSchrijver = csvSchrijver;
        }

        /// <summary>

        /// </summary>
        public void VoegProjectToe(Project project)
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
            _repo.VoegProjectToe(project);
        }

        /// <summary>

        /// </summary>
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

        /// <summary>
        
        /// </summary>
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

        /// <summary>
        
        /// </summary>
        public void MaakProjectFiche(int id, string pad)
        {
            var project = _repo.GeefProject(id); 

            if (project == null)
            {
                throw new ProjectException("Project niet gevonden.");
            }

            try
            {
                
               
                var projectLijst = new List<Project> { project };
                
                byte[] pdfData = _pdfSchrijver.MaakPDF(projectLijst);

                System.IO.File.WriteAllBytes(pad, pdfData);
            }
            catch (Exception ex)
            {
                throw new ProjectException("Fout bij het maken van de projectfiche: " + ex.Message);
            }
        }

        /// <summary>
        //
        /// </summary>
        public List<ProjectCombinatie> GeefAlleProjecten()
        {
            return _repo.GeefAlleProjecten();
        }
    }
    
}
