using FFXIV_RaidLootAPI.Data;
using FFXIV_RaidLootAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FFXIV_RaidLootAPI.DTO;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;

namespace FFXIV_RaidLootAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigins")]
    public class GearController : ControllerBase
    {
        private readonly IDbContextFactory<DataContext> _context;

        public GearController(IDbContextFactory<DataContext> context)
        {
            _context = context;
        }


        [HttpPost("PopulateDatabase")]
        public async Task<ActionResult> PopulateDatabase(int StartingIndex, int EndIndex){
            using (var context = _context.CreateDbContext())
            {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://etro.gg");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                for (int i = StartingIndex;i<=EndIndex;i++)
                {

                        try
                        {
                            // Make the GET request
                            HttpResponseMessage response = await client.GetAsync("/api/equipment/"+i.ToString()+"/");
                            // Ensure the request was successful
                            //response.EnsureSuccessStatusCode();
                            
                            // Read and process the response content
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Dictionary<string, object>? responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

                            if (responseData is null || responseData.ContainsKey("detail"))
                            {
                                Console.WriteLine(i.ToString() + " : " + responseData?["detail"].ToString());
                                continue;
                            }
                            Console.WriteLine("Valid id : " + i.ToString());

                            

                            string GearILevel = responseData["itemLevel"].ToString();
                            string GearName = responseData["name"].ToString();
                            string JobName = responseData["jobName"].ToString();
                            string IconPath = responseData["iconPath"].ToString();
                            if (int.Parse(GearILevel) < 690 || GearName.Contains("Shield"))
                                continue;
                            bool IsWeapon = Convert.ToBoolean(responseData["weapon"].ToString());

                            Gear newGear = Gear.CreateGearFromEtro(GearILevel,GearName,IsWeapon,JobName,IconPath, i);

                            if (newGear.GearType == GearType.LeftRing)
                            {   
                                Gear RightRingGear = new Gear() 
                                {
                                    Name=newGear.Name + " (R)",
                                    GearLevel=newGear.GearLevel,
                                    GearStage=newGear.GearStage,
                                    GearType=GearType.RightRing,
                                    GearCategory=newGear.GearCategory,
                                    GearWeaponCategory=Job.Empty,
                                    IconPath=newGear.IconPath,
                                    EtroGearId=newGear.EtroGearId
                                };
                                newGear.Name += " (L)";
                                GearName += " (L)";
                                await context.Gears.AddAsync(RightRingGear);
                            }
                            context.Gears.Add(newGear);
                            await context.SaveChangesAsync();

                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("Request error: " + e.Message + " : " + i.ToString());
                            return Ok("Could not find etro gearset.");
                        }
                }
            
            return Ok();
            }
            }
        }

        [HttpPost("UpdateDatabase")]
        public async Task<ActionResult> UpdateDatabase(int StartingIndex, int EndIndex){
            using (var context = _context.CreateDbContext())
            {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://etro.gg");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                for (int i = StartingIndex;i<=EndIndex;i++)
                {

                        try
                        {
                            // Make the GET request
                            HttpResponseMessage response = await client.GetAsync("/api/equipment/"+i.ToString()+"/");
                            // Ensure the request was successful
                            //response.EnsureSuccessStatusCode();
                            
                            // Read and process the response content
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Dictionary<string, object>? responseData = JsonSerializer.Deserialize<Dictionary<string, object>>(responseBody);

                            if (responseData is null || responseData.ContainsKey("detail"))
                            {
                                Console.WriteLine(i.ToString() + " : " + responseData?["detail"].ToString());
                                continue;
                            }
                            Console.WriteLine("Valid id : " + i.ToString());

                            

                            string GearILevel = responseData["itemLevel"].ToString();
                            string GearName = responseData["name"].ToString();
                            string JobName = responseData["jobName"].ToString();
                            string IconPath = responseData["iconPath"].ToString();
                            if (int.Parse(GearILevel) < 690 || GearName.Contains("Shield"))
                                continue;
                            bool IsWeapon = Convert.ToBoolean(responseData["weapon"].ToString());

                            Gear newGear = Gear.CreateGearFromEtro(GearILevel,GearName,IsWeapon,JobName,IconPath, i);

                            if (newGear.GearType == GearType.LeftRing)
                            {   
                                Gear RightRingGear = new Gear() 
                                {
                                    Name=newGear.Name + " (R)",
                                    GearLevel=newGear.GearLevel,
                                    GearStage=newGear.GearStage,
                                    GearType=GearType.RightRing,
                                    GearCategory=newGear.GearCategory,
                                    GearWeaponCategory=Job.Empty,
                                    IconPath=newGear.IconPath,
                                    EtroGearId=newGear.EtroGearId
                                };
                                newGear.Name += " (L)";
                                GearName += " (L)";
                                Gear? existingGear = await context.Gears.FirstOrDefaultAsync(g => g.GearLevel == RightRingGear.GearLevel && g.GearStage == RightRingGear.GearStage && g.GearType == RightRingGear.GearType && g.GearCategory == RightRingGear.GearCategory);
                                if (existingGear is not null)
                                {
                                    existingGear.EtroGearId = newGear.EtroGearId;
                                    existingGear.IconPath = newGear.IconPath;
                                    existingGear.Name = RightRingGear.Name;
                                    await context.SaveChangesAsync();
                                    continue;
                                } else{
                                    Console.WriteLine("Gear not found: " + RightRingGear.Name + " : " + RightRingGear.GearLevel + " : " + RightRingGear.GearStage + " : " + RightRingGear.GearType + " : " + RightRingGear.GearCategory);
                                }
                            }
                                Gear? gear2 = await context.Gears.FirstOrDefaultAsync(g => g.GearLevel == newGear.GearLevel && g.GearStage == newGear.GearStage && g.GearType == newGear.GearType && g.GearCategory == newGear.GearCategory);
                                if (gear2 is not null)
                                {
                                    gear2.EtroGearId = newGear.EtroGearId;
                                    gear2.IconPath = newGear.IconPath;
                                    gear2.Name = newGear.Name;
                                    await context.SaveChangesAsync();
                                    continue;
                                }else{
                                    Console.WriteLine("Gear not found: " + newGear.Name + " : " + newGear.GearLevel + " : " + newGear.GearStage + " : " + newGear.GearType + " : " + newGear.GearCategory);
                                }

                        }
                        catch (HttpRequestException e)
                        {
                            Console.WriteLine("Request error: " + e.Message + " : " + i.ToString());
                            return Ok("Could not find etro gearset.");
                        }
                }
            
            return Ok();
            }
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Gear>>> GetAllGear()
        {
            using (var context = _context.CreateDbContext())
            {
                List<Gear> gearList = await context.Gears.ToListAsync();
                return Ok(gearList);
            }
            
        }
        [HttpGet("GetGearOption/{Job}/{GearType}")]
        public async Task<ActionResult<GearOptionsDTO>> GetGearOption(Job Job, GearType GearType)
        {
        // THIS FUNCTION HAS BEEN REWRITTEN AS A GEAR CLASS FUNCTION
        // GEAR.GetGearOptions()
        using (var context = _context.CreateDbContext())
        {
            List<GearOptionsDTO.GearOption> OptionList = new List<GearOptionsDTO.GearOption>();
            if (GearType == GearType.Weapon)
            {
                IEnumerable<Gear> GearIterFromDb = context.Gears.Where(g => g.GearWeaponCategory == Job && g.GearCategory == GearCategory.Weapon);
                foreach (Gear gear in GearIterFromDb)
                {
                    OptionList.Add(new GearOptionsDTO.GearOption()
                    {
                        GearName = gear.Name,
                        GearItemLevel = gear.GearLevel,
                        GearStage = gear.GearStage.ToString(),
                        GearId = gear.Id
                    });
                }
            }
            else
            {
                GearCategory GearToChooseFrom = Gear.JOB_TO_GEAR_CATEGORY_MAP[Job][(int) GearType >=7 ? 1 : 0];
                // Left side is index 0 right side is index 1
                IEnumerable<Gear> GearIterFromDb = context.Gears.Where(g => g.GearCategory == GearToChooseFrom && g.GearType == GearType);
                foreach (Gear gear in GearIterFromDb)
                {
                    OptionList.Add(new GearOptionsDTO.GearOption()
                    {
                        GearName = gear.Name,
                        GearItemLevel = gear.GearLevel,
                        GearStage = gear.GearStage.ToString(),
                        GearId = gear.Id
                    });
                }

            }
            return Ok(new GearOptionsDTO() 
            {
                GearOptionList=OptionList
            });
        }
        }
    }
}
