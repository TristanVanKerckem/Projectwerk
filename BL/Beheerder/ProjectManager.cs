using ProjectbeheerBL.Domein;
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
                throw new ProjectException("Fout bij het exporteren van projecten naar CSV: " + ex.Message);
            }
        }



        public void MaakProjectFiche(int id, string pad)
        {
            // Ophalen van projectcombinaties volgens de interface
            var projecten = _repo.GeefAlleProjecten();

            if (projecten == null || !projecten.Any(p => p.Id == id))
            {
                throw new ProjectException("Project niet gevonden.");
            }

            try
            {
                // De interface IPDFschrijver verwacht een List<ProjectCombinatie>
                // We geven de volledige lijst mee (of een gefilterde lijst)
                byte[] pdfData = _pdfSchrijver.MaakPDF(projecten);

                // Schrijf de byte array naar een bestand op het opgegeven pad
                System.IO.File.WriteAllBytes(pad, pdfData);
            }
            catch (Exception ex)
            {
                throw new ProjectException("Fout bij het maken van de projectfiche: " + ex.Message);
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
        //        _pdfSchrijver.MaakPDF(projecten);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new ProjectException("Fout bij het maken van de projectfiche: " + ex.Message);
        //    }
        //}
    }
}
