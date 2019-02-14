using BookingApp.Helpers;
using BookingApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BookingApp.Data
{
    public class DbInitializer
    {
        readonly ApplicationDbContext context;
        readonly UserManager<ApplicationUser> userManager;
        readonly RoleManager<IdentityRole> roleManager;

        ApplicationUser superAdmin;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        /// <summary>
        /// Initializes persistent storage with at least single SuperAdmin (at production). 
        /// For debugging, adds dummy users and data.
        /// <para>Enable context.Database.EnsureDeleted() to force full reseeding with each run.</para>
        /// </summary>
        public async Task Initialize()
        {
#if DEBUG
            // WARNING! Wipes the entire database.
            //context.Database.EnsureDeleted();
#endif
            //make sure DB is created
            bool isDbFresh = context.Database.EnsureCreated();

            await EnsureCompetentSuperAdmin();

            if (isDbFresh)
            {
                await CreateStoreProceduresInDb();
#if DEBUG
                await SeedDummyData();
#endif
            }
        }

        /// <summary>
        /// Creating/Granting competency for the SuperAdmin.
        /// </summary>
        /// <returns></returns>
        async Task EnsureCompetentSuperAdmin()
        {
            //make sure we have basic roles
            if (!await roleManager.RoleExistsAsync(RoleTypes.Admin))
                await roleManager.CreateAsync(new IdentityRole(RoleTypes.Admin));
            if (!await roleManager.RoleExistsAsync(RoleTypes.User))
                await roleManager.CreateAsync(new IdentityRole(RoleTypes.User));

            //making sure we have competent SuperAdmin
            var superAdminEmail = "superadmin@admin.cow";  //temporary: replace with the real SuperAdmin email address

            superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

            if (superAdmin == null)//create new if no luck
            {
                superAdmin = new ApplicationUser() { UserName = "SuperAdmin", Email = superAdminEmail };
                await userManager.CreateAsync(user: superAdmin, password: superAdmin.UserName);//temporary: discard password once SuperAdmin has real email address
            }

            //ensuring SuperAdmin is entitled to all roles
            if (!await userManager.IsInRoleAsync(superAdmin, RoleTypes.Admin))
                await userManager.AddToRoleAsync(superAdmin, RoleTypes.Admin);

            if (!await userManager.IsInRoleAsync(superAdmin, RoleTypes.User))
                await userManager.AddToRoleAsync(superAdmin, RoleTypes.User);

            //unblocking and approving SuperAdmin
            superAdmin.ApprovalStatus = true;
            superAdmin.IsBlocked = false;
            await userManager.UpdateAsync(superAdmin);
        }


        async Task SeedDummyData()
        {
            var dummyUsernames = new[] { "Tiger", "Elephant", "Lion", "Bear", "Cheetah", "Wolf", "Camel", "Eagle", "Mantis" };

            #region Dummy data source
            var rand = new Random();
            const string loremIpsum = "The core of the city consists predominately of wall-to-wall buildings, with blocks of clustered low-rises made out of a variety of old and new buildings. Under Combine rule, certain residential buildings in the city are used as accommodations for citizens. Conditions in such housings are typically seen as poor, with very few luxuries and constant inspection and raids by Civil Protection. However, some city infrastructure, such as power plants, are maintained by the Combine, and electricity is made widely available from both traditional sources and Combine generators. The Combine themselves occupied some former government buildings, such as the Overwatch Nexus, to help keep control over the city. The city was large enough to provide all necessary needs for the citizens before the Combines occupation. This is supported by the presence of a hospital, several cafés and restaurants, office buildings, and underground city systems; most of which are still intact but abandoned. The outskirts of City 17 features industrial districts and additional Soviet - style housing, most of which are considered off - limits to citizens.The industrial districts are seen linked to the city via railway lines and canals. As there was little emphasis in maintaining non - essential parts of the city, many areas of City 17 suffered from urban decay prior to the Citadel's explosion.";
            #endregion

            #region Users
            var users = new Dictionary<string, ApplicationUser> { { superAdmin.UserName, superAdmin } };

            for (int i = 0; i < dummyUsernames.Length; i++)
            {
                string name = dummyUsernames[i];
                string password = name;// password == name
                bool isAdmin = i < 2;// first two are admins

                bool? approvalStatus = null; // newcomers

                if (i < 6) // approves
                    approvalStatus = true;

                if (i == 3 || i == 7) // rejects
                    approvalStatus = false;

                bool isBlocked = (i == 3);//active everyone except fourth
                string email = $"{name}@{(isAdmin ? RoleTypes.Admin : RoleTypes.User)}.cow".ToLower();//e.g. lion@admin.cow & camel@user.cow

                var user = new ApplicationUser() { UserName = name, Email = email };
                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, RoleTypes.User);
                if (isAdmin)
                    await userManager.AddToRoleAsync(user, RoleTypes.Admin);

                user.IsBlocked = isBlocked;
                user.ApprovalStatus = approvalStatus;
                await userManager.UpdateAsync(user);

                users.Add(user.UserName, user);
            }
            #endregion

            #region Rules
            var rules = new Dictionary<string, Rule> {
                { "Defaultest",      new Rule() { Title = "Defaultest",    MinTime = 1,  MaxTime = 1440, } },
                { "Rooms",           new Rule() { Title = "Rooms",         MinTime = 60, MaxTime = 480, StepTime = 30, ServiceTime = 0,   ReuseTimeout = 0,   PreOrderTimeLimit = 1440 } },
                { "Teslas",          new Rule() { Title = "Teslas",        MinTime = 60, MaxTime = 300, StepTime = 30, ServiceTime = 180, ReuseTimeout = 0,   PreOrderTimeLimit = 360 } },
                { "Toilets",         new Rule() { Title = "Toilets",       MinTime = 1,  MaxTime = 15,  StepTime = 1,  ServiceTime = 0,   ReuseTimeout = 240, PreOrderTimeLimit = 120 } },
                { "Strict Stuff",    new Rule() { Title = "Strict Stuff",  MinTime = 30, MaxTime = 80,  StepTime = 10, ServiceTime = 40,  ReuseTimeout = 300, PreOrderTimeLimit = 1200 } },
                { "Inactive Rule",   new Rule() { Title = "Inactive Rule", MinTime = 30, MaxTime = 80,  StepTime = 10, ServiceTime = 40,  ReuseTimeout = 300, PreOrderTimeLimit = 1200, IsActive = false } }
            };
            rules.ToList().ForEach(e => e.Value.Creator = e.Value.Updater = superAdmin);

            //pushing into EF
            context.Rules.AddRange(rules.Select(e => e.Value));
            #endregion

            #region TreeGroups
            var treeGroups = new Dictionary<string, TreeGroup> {
                { "Town Hall", new TreeGroup() { Title = "Town Hall" } },
                { "Bike Rental", new TreeGroup() { Title = "Bike Rental" } }
            };
            treeGroups.Add("Spire Balcony", new TreeGroup() { Title = "Spire Balcony", ParentTreeGroup = treeGroups["Town Hall"], DefaultRule = rules["Inactive Rule"] });
            treeGroups.Add("Gala Balcony", new TreeGroup() { Title = "Gala Balcony", ParentTreeGroup = treeGroups["Town Hall"] });
            treeGroups.Add("Museums", new TreeGroup() { Title = "Museums", ParentTreeGroup = treeGroups["Town Hall"], IsActive = false });

            treeGroups.ToList().ForEach(e => e.Value.Creator = e.Value.Updater = superAdmin);

            context.TreeGroups.AddRange(treeGroups.Select(e => e.Value));
            #endregion

            #region Resources
            var resources = new Dictionary<int, Resource> {
                {  1, new Resource() { Title = "Nothern View",          TreeGroup = treeGroups["Spire Balcony"], Rule = rules["Defaultest"] } },
                {  2, new Resource() { Title = "Southern View",         TreeGroup = treeGroups["Spire Balcony"], Rule = rules["Toilets"] } },
                {  3, new Resource() { Title = "Flag",                  TreeGroup = treeGroups["Spire Balcony"], Rule = rules["Strict Stuff"] } },

                {  4, new Resource() { Title = "Trumpet Ensemble",      TreeGroup = treeGroups["Gala Balcony"],  Rule = rules["Teslas"] } },

                {  5, new Resource() { Title = "First Floor Hallway",   TreeGroup = treeGroups["Town Hall"],     Rule = rules["Toilets"] } },

                {  6, new Resource() { Title = "Natural Museum",        TreeGroup = treeGroups["Museums"],       Rule = rules["Rooms"], IsActive = false } },
                {  7, new Resource() { Title = "Art Museum",            TreeGroup = treeGroups["Museums"],       Rule = rules["Rooms"], IsActive = false } },
                {  8, new Resource() { Title = "History Museum",        TreeGroup = treeGroups["Museums"],       Rule = rules["Rooms"] } },

                {  9, new Resource() { Title = "Civil Defence Alarm",                                            Rule = rules["Strict Stuff"] } },

                { 10, new Resource() { Title = "Cruiser Bicycle #2000", TreeGroup = treeGroups["Bike Rental"],   Rule = rules["Inactive Rule"] } },
                { 11, new Resource() { Title = "Cruiser Bicycle #46",   TreeGroup = treeGroups["Bike Rental"],   Rule = rules["Teslas"] } },
                { 12, new Resource() { Title = "Ukraine Tier0 Bicycle", TreeGroup = treeGroups["Bike Rental"],   Rule = rules["Toilets"] } },
                { 13, new Resource() { Title = "Mountain Bike Roger",   TreeGroup = treeGroups["Bike Rental"],   Rule = rules["Teslas"], IsActive = false } },
            };

            //fill authorship & description (random stuff)
            resources.ToList().ForEach(e =>
            {
                e.Value.Creator = e.Value.Updater = superAdmin;
                e.Value.Description = loremIpsum.Substring(
                    rand.Next((int)(loremIpsum.Length * 0.7)),
                    rand.Next((int)(loremIpsum.Length * 0.1), (int)(loremIpsum.Length * 0.25))
                );
            });

            //pushing into EF
            context.Resources.AddRange(resources.Select(e => e.Value));
            #endregion

            //saving changes to DB before seeding booking, so the rules have default storage values.
            context.SaveChanges();

            //SeedBookingsSimple(rand, loremIpsum, users, resources);
            SeedBookingsNonOverlapping(rand, loremIpsum, users, resources);

            //saving changes to DB
            context.SaveChanges();
        }

        /// <summary>
        /// Seeds booking in a simple manner. The bookings do not follow the booking rules and may overlap, causing same-time booking conflict.
        /// </summary>
        private void SeedBookingsSimple(Random rand, string loremIpsum, Dictionary<string, ApplicationUser> users, Dictionary<int, Resource> resources)
        {
            var bookings = new List<Booking>();
            for (int i = 0; i < 50; i++)
            {
                var booking = new Booking
                {
                    Note = loremIpsum.Substring(rand.Next(loremIpsum.Length - 200), rand.Next(0, 64)).Trim(),
                    Resource = resources[rand.Next(1, resources.Count)],
                    StartTime = DateTime.Now + TimeSpan.FromMinutes(rand.Next(-1440 * 3, +1440 * 2)),
                    Creator = users.OrderBy(e => rand.Next()).First().Value
                };
                booking.EndTime = booking.StartTime + TimeSpan.FromMinutes(rand.Next(booking.Resource.Rule.MinTime ?? 1, booking.Resource.Rule.MaxTime ?? 1440));
                booking.Updater = booking.Creator;

                bookings.Add(booking);
            }

            //cancel out some bookings
            foreach (var booking in bookings.OrderBy(b => rand.Next()).Take(bookings.Count() / 5).ToList())
            {
                var deviationMagnitude = booking.Resource.Rule.MaxTime ?? 360;
                var deviation = TimeSpan.FromMinutes(rand.Next(-deviationMagnitude, +deviationMagnitude));
                booking.TerminationTime = booking.StartTime + deviation;
            }

            //pushing into EF
            context.Bookings.AddRange(bookings);
        }

        /// <summary>
        /// Seeds bookings without overlaps. For each resource, fill bookings from the end.
        /// </summary>
        private void SeedBookingsNonOverlapping(Random rand, string loremIpsum, Dictionary<string, ApplicationUser> users, Dictionary<int, Resource> resources)
        {
            const int day = 60 * 24;
            const int historyWriteMinutesLimit = -day;
            const double terminationChance = 0.2;

            foreach (var entry in resources)
            {
                Resource r = entry.Value;

                #region Per-resource variables
                int stepMinutes = (int)r.Rule.StepTime;
                int preOrderLimit = r.Rule.PreOrderTimeLimit < 1 ? day : (int)r.Rule.PreOrderTimeLimit;

                int maxMinutes =  (int)r.Rule.MaxTime;
                int maxSteps = maxMinutes / stepMinutes;

                int minMinutes = (int)r.Rule.MinTime;
                int minSteps = minMinutes / stepMinutes;

                int serviceMinutes = (int)r.Rule.ServiceTime;

                int postUsageDelayMinutesLimit = rand.Next(maxMinutes, day);
                #endregion

                // The value used in a loop, for correct time determination
                int earliestOccupiedMinute = preOrderLimit + maxMinutes;

                var perResourceBookings = new List<Booking>();

                while(earliestOccupiedMinute > historyWriteMinutesLimit)
                {
                    int busyPlanMinutes = rand.Next(minSteps, maxSteps + 1) * stepMinutes;
                    int postUsageMinutesDelay = rand.Next(postUsageDelayMinutesLimit);

                    int plannedEndMinutes = earliestOccupiedMinute - serviceMinutes - postUsageMinutesDelay;
                    int? factualEndMinutes = plannedEndMinutes;

                    int plannedStartMinutes = plannedEndMinutes - busyPlanMinutes;
                    int? factualStartMinutes = plannedStartMinutes;

                    bool terminationHappened = rand.Next((int)(1/terminationChance)) == 0;
                    int? terminatedAtMinutes = null;

                    if (terminationHappened)
                    {
                        terminatedAtMinutes = plannedEndMinutes - rand.Next(preOrderLimit / 2);

                        bool isPreCancelled = terminatedAtMinutes <= plannedStartMinutes;

                        if (isPreCancelled)
                            factualStartMinutes = factualEndMinutes = null;
                        else
                            factualEndMinutes = terminatedAtMinutes; 
                    }

                    earliestOccupiedMinute = factualStartMinutes ?? earliestOccupiedMinute;

                    DateTime now = DateTime.Now;
                    var creator = users.OrderBy(e => rand.Next()).First().Value;
                    var startTime = now + TimeSpan.FromMinutes(plannedStartMinutes);
                    var terminationTime = terminatedAtMinutes == null ? (DateTime?)null : now + TimeSpan.FromMinutes((int)terminatedAtMinutes);
                    var createdTime = startTime - TimeSpan.FromMinutes(preOrderLimit * rand.Next(100) / 100);
                    var updatedTime = terminationTime ?? createdTime;

                    perResourceBookings.Add(
                        new Booking
                        {
                            Note = loremIpsum.Substring(rand.Next(loremIpsum.Length - 200), rand.Next(0, 64)).Trim(),
                            Resource = r,
                            StartTime = startTime,
                            EndTime = now + TimeSpan.FromMinutes(plannedEndMinutes),
                            TerminationTime = terminationTime,
                            Creator = creator,
                            Updater = new[] { creator, users.First().Value }[rand.Next(2)],
                            CreatedTime = createdTime,
                            UpdatedTime = updatedTime
                        }
                    );
                }

                //pushing into EF
                perResourceBookings.Reverse();
                context.Bookings.AddRange(perResourceBookings);
            }
        }

        /// <summary>
        /// Search store procedures code as embeded ressources in 
        /// namespace <c>Data.StoredProcedures</c> and must be with extension sql
        /// </summary>
        private async Task CreateStoreProceduresInDb()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var storedProcedureResourceList = assembly.GetManifestResourceNames().Where(
                str => str.Contains("Data.StoredProcedures") && str.EndsWith(".sql")
                );

            foreach (var resourceName in storedProcedureResourceList)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    await context.Database.ExecuteSqlCommandAsync(reader.ReadToEnd());
                }
            }
        }
    }
}