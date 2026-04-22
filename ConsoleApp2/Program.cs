using System;
using System.Collections.Generic;
using ProjectbeheerDL.Repository;
using ProjectbeheerBL.Beheerder;
using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using System.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Connection string (Pas aan naar jouw database)
            string connectionString = @"Data Source=DESKTOP-A774TOE;Initial Catalog=projectwerkpg;Integrated Security=True;TrustServerCertificate=True";

            //// 2. Instantieer de repo en manager
            //// We gebruiken 'ProjectRepo' die 'IProjectRepository' implementeert.
            //var repo = new ProjectRepo(connectionString);
            //var manager = new ProjectManager(repo, null, null);

            //Console.WriteLine("=== Start Database Test ===\n");

            //try
            //{
            //    // TEST 1: Alle projecten ophalen
            //    Console.WriteLine("Test 1: Projecten ophalen...");
            //    var alleCombinaties = manager.GeefAlleProjecten();
            //    Console.WriteLine($"Aantal combinaties gevonden: {alleCombinaties.Count}");

            //    foreach (var combo in alleCombinaties)
            //    {
            //        foreach (var p in combo.ProjectComboLijst)
            //        {
            //            // OPLOSSING CS1061: Gebruik 'ProjectLocatie' of 'Locatie' afhankelijk van je Project klasse[cite: 1, 6].
            //            // Volgens source 9 heet de property in de database 'locatieId'.
            //            Console.WriteLine($"- Project: {p.Titel} (ID: {p.Id})");

            //            if (p.Locatie != null) // Pas aan naar 'p.Locatie' als 'ProjectLocatie' niet bestaat.
            //            {
            //                Console.WriteLine($"  Locatie: {p.Locatie.Straat}, {p.Locatie.Gemeente}");
            //            }
            //        }
            //    }

            //    // TEST 2: Een specifiek project ophalen
            //    // OPLOSSING CS1061: In ProjectManager_2 heet de methode 'GeefProject' of gebruik de repo direct.
            //    Console.WriteLine("\nTest 2: Details ophalen voor project ID 1...");
            //    var project = repo.GeefProject(1);

            //    if (project != null)
            //    {
            //        Console.WriteLine($"Gevonden: {project.Titel}");
            //        Console.WriteLine($"Beschrijving: {project.Beschrijving}");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Project met ID 1 niet gevonden in de database.");
            //    }

            //    Console.WriteLine("\n=== Test voltooid! ===");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"\n!!! FOUT: {ex.Message}");
            //    if (ex.InnerException != null) Console.WriteLine($"Inner: {ex.InnerException.Message}");
            //}

            //Console.ReadLine();

            //var repo = new ProjectRepo(connectionString);
            //var manager = new ProjectManager(repo, null, null);

            //Console.WriteLine("========================================");
            //Console.WriteLine("   COMPLETE SYSTEM DIAGNOSTICS");
            //Console.WriteLine("========================================\n");

            //try
            //{
            //    // 1. اختبار جلب المشاريع
            //    Console.WriteLine("[TEST 1] GeefAlleProjecten:");
            //    var combinaties = manager.GeefAlleProjecten();

            //    foreach (var combo in combinaties)
            //    {
            //        Console.WriteLine($"\n> Combinatie ID: {combo.Id}");
            //        foreach (var p in combo.ProjectComboLijst)
            //        {
            //            Console.WriteLine($"  - Project: {p.Titel.ToUpper()} (Status: {p.Status})");

            //            // الوصول للشركاء عبر المفاتيح (Keys) في الـ Dictionary
            //            if (p.ProjectPartners != null && p.ProjectPartners.Any())
            //            {
            //                string partnerNamen = string.Join(", ", p.ProjectPartners.Keys.Select(k => k.Naam));
            //                Console.WriteLine($"    Partners: {partnerNamen}");
            //            }

            //            // تصحيح الأنواع بناءً على الخصائص الفعلية في الكود الخاص بك
            //            if (p is Stadsontwikkeling so)
            //            {
            //                Console.WriteLine($"    Type: STADSONTWIKKELING");
            //                // تأكد أن VergunningStatus موجودة في كلاس StadsOntwikkeling
            //                Console.WriteLine($"    Toegankelijkheid: {so.Toegankelijkheid}");
            //            }
            //            else if (p is GroeneRuimte gr)
            //            {
            //                Console.WriteLine($"    Type: GROENE RUIMTE");
            //                // تم تغيير Grootte إلى Oppervlakte كما في الكود الخاص بك
            //                Console.WriteLine($"    Oppervlakte: {gr.Oppervlakte} m²");
            //                Console.WriteLine($"    Biodiversiteit: {gr.Biodiversiteit}/10");
            //            }
            //        }
            //    }

            //    // 2. إحصائيات سريعة
            //    Console.WriteLine("\n[TEST 2] Statistics:");
            //    int totalProjects = combinaties.SelectMany(c => c.ProjectComboLijst).Count();
            //    Console.WriteLine($"  Totaal aantal projecten: {totalProjects}");

            //    Console.WriteLine("\n========================================");
            //    Console.WriteLine("        TEST SUCCESVOL VOLTOOID");
            //    Console.WriteLine("========================================");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"\n!!! FOUT GEVONDEN: {ex.Message}");
            //}

            //Console.WriteLine("\nPress Enter to exit...");
            //Console.ReadLine();


            var repo = new ProjectRepo(connectionString);
            var manager = new ProjectManager(repo, null, null);

            Console.WriteLine("========================================");
            Console.WriteLine("    ULTIMATE PROJECT & PARTNER VIEW");
            Console.WriteLine("========================================\n");

            try
            {
                // 1. جلب كافة البيانات[cite: 1]
                var combinaties = manager.GeefAlleProjecten();
                var allProjects = combinaties.SelectMany(c => c.ProjectComboLijst).ToList();

                Console.WriteLine($"[INFO] Total Projects Loaded: {allProjects.Count}\n");

                // 2. عرض المشاريع مع الشركاء والتفاصيل[cite: 1]
                foreach (var p in allProjects)
                {
                    Console.WriteLine($"[PROJECT] ID: {p.Id} | {p.Titel.ToUpper()}");
                    Console.WriteLine($"   Status: {p.Status} | Locatie: {p.Locatie.Gemeente} ({p.Locatie.Wijk})");

                    // إضافة سطر الوصف
                    Console.WriteLine($"   Beschrijving: {p.Beschrijving}");

                    // التأكد من حلقة الشركاء
                    if (p.ProjectPartners != null && p.ProjectPartners.Any())
                    {
                        foreach (var entry in p.ProjectPartners)
                        {
                            Console.WriteLine($"   Partner: {entry.Key.Naam} | Role: {string.Join(", ", entry.Value)}");
                        }
                    }
                    // عرض الشركاء من الـ Dictionary[cite: 1]
                    //if (p.ProjectPartners != null && p.ProjectPartners.Any())
                    //{
                    //    Console.Write("   Partners: ");
                    //    var partnerDetails = p.ProjectPartners.Select(entry => $"{entry.Key.Naam} (Role: {string.Join("/", entry.Value)})");
                    //    Console.WriteLine(string.Join(" | ", partnerDetails));
                    //}
                    else
                    {
                        Console.WriteLine("   Partners: NONE ASSIGNED (Check your Repository Join query)");
                    }

                    // تفاصيل النوع[cite: 1]
                    if (p is GroeneRuimte gr)
                        Console.WriteLine($"   Extra: Groene Ruimte | Oppervlakte: {gr.Oppervlakte} m² | Biodiversiteit: {gr.Biodiversiteit}");
                    else if (p is Stadsontwikkeling so)
                        Console.WriteLine($"   Extra: Stadsontwikkeling | Toegankelijkheid: {so.Toegankelijkheid}");

                    Console.WriteLine(new string('-', 50));
                }


                Console.WriteLine("\n" + new string('=', 40));
                Console.WriteLine("        ADVANCED FILTER RESULTS");
                Console.WriteLine(new string('=', 40));


                string filterCity = "Gent";
                var cityResults = allProjects.Where(p => p.Locatie.Gemeente.Equals(filterCity, StringComparison.OrdinalIgnoreCase)).ToList();
                Console.WriteLine($"\n> Projects in '{filterCity}': {cityResults.Count}");


                var withPartners = allProjects.Where(p => p.ProjectPartners.Any()).ToList();
                Console.WriteLine($"> Projects with assigned partners: {withPartners.Count}");


                string partnerName = "Build-It NV";
                var specificPartner = allProjects.Where(p => p.ProjectPartners.Keys.Any(k => k.Naam.Contains(partnerName))).ToList();
                Console.WriteLine($"> Projects involving '{partnerName}': {specificPartner.Count}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[ERROR] {ex.Message}");
            }

            Console.WriteLine("\nDone. Press Enter to exit...");
            Console.ReadLine();
        }
    }

}

