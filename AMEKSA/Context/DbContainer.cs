using AMEKSA.Privilage;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AMEKSA.Entities;

namespace AMEKSA.Context
{
    public class DbContainer : IdentityDbContext<ExtendIdentityUser,ExtendIdentityRole,string>
    {
        public DbContainer(DbContextOptions<DbContainer> ops):base(ops)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<ExtendIdentityUser>().HasMany<UserSubmanager>(a => a.Reps).WithOne(a => a.Rep);
            builder.Entity<ExtendIdentityUser>().HasMany<UserSubmanager>(a => a.Managers).WithOne(a => a.Manager);
            builder.Entity<ContactMedicalVisit>().HasOne(a => a.Manager).WithMany(a => a.contactmedicalvisit2);
            builder.Entity<AccountMedicalVisit>().HasOne(a => a.Manager).WithMany(a => a.accountmedicalvisit2);
            builder.Entity<AccountSalesVisit>().HasOne(a => a.Manager).WithMany(a => a.accountsalesvisit2);
            builder.Entity<AccountSupportiveVisit>().HasOne(a => a.Manager).WithMany(a => a.accountsupportivevisit2);

        }

        public DbSet<Account> account { get; set; }

        public DbSet<AccountBrandPayment> accountBrandPayment { get; set; }

        public DbSet<AccountMedicalVisit> accountMedicalVisit { get; set; }

        public DbSet<AccountMedicalVisitProducts> accountMedicalVisitProducts { get; set; }

        public DbSet<AccountSalesVisit> accountSalesVisit { get; set; }

        public DbSet<AccountSalesVisitBrand> accountSalesVisitBrand { get; set; }

        public DbSet<AccountType> accountType { get; set; }

        public DbSet<Brand> brand { get; set; }

        public DbSet<Category> category { get; set; }

        public DbSet<City> city { get; set; }

        public DbSet<Contact> contact { get; set; }

        public DbSet<ContactMedicalVisit> contactMedicalVisit { get; set; }

        public DbSet<ContactMedicalVisitProduct> contactMedicalVisitProduct { get; set; }

        public DbSet<ContactType> contactType { get; set; }

        public DbSet<District> district { get; set; }

        public DbSet<Menu> menu { get; set; }

        public DbSet<Notifications> notifications { get; set; }

        public DbSet<Product> product { get; set; }

        public DbSet<PurchaseType> purchaseType { get; set; }

        public DbSet<RoleMenu> roleMenu { get; set; }

        public DbSet<UserAccount> userAccount { get; set; }

        public DbSet<UserBrand> userBrand { get; set; }

        public DbSet<UserContact> userContact { get; set; }

        public DbSet<AccountMedicalVisitPerson> accountMedicalVisitPerson { get; set; }

        public DbSet<AccountSalesVisitPerson> accountSalesVisitPerson { get; set; }

        public DbSet<SalesAid> salesAid { get; set; }

        public DbSet<ContactSalesAid> contactSalesAid { get; set; }

        public DbSet<OpenningRequest> openningRequest { get; set; }

        public DbSet<RequestDeleteAccountMedical> requestDeleteAccountMedical { get; set; }

        public DbSet<RequestDeleteContactMedical> requestDeleteContactMedical { get; set; }

        public DbSet<RequestDeleteAccountSales> requestDeleteAccountSales { get; set; }

        public DbSet<RequestChangeContactTarget> requestChangeContactTargets { get; set; }

        public DbSet<AccountMedicalVisitChat> accountMedicalVisitChat { get; set; }

        public DbSet<AccountSalesVisitChat> accountSalesVisitChat { get; set; }

        public DbSet<ContactMedicalVisitChat> contactMedicalVisitChat { get; set; }

        public DbSet<UserTimeOff> userTimeOff { get; set; }

        public DbSet<TimeOffTerritoryReasons> timeOffTerrirtoryReasons { get; set; }

        public DbSet<WorkingDays> workingDays { get; set; }

        public DbSet<VacancyRequests> vacancyRequests { get; set; }

        public DbSet<ContactMonthlyPlan> contactMonthlyPlan { get; set; }

        public DbSet<AccountMonthlyPlan> accountMonthlyPlan { get; set; }

        public DbSet<Entities.Properties> properties { get; set; }

        public DbSet<AccountSalesVisitCollection> accountSalesVisitCollection { get; set; }

        public DbSet<AccountMonthlyPlanCollection> accountMonthlyPlanCollection { get; set; }

        public DbSet<AccountBalance> accountBalance { get; set; }

        public DbSet<RequestChangeContactCategory> requestChangeCategory { get; set; }

        public DbSet<Attend> attend { get; set; }

        public DbSet<StringProperties> stringProperties { get; set; }

        public DbSet<TravelCities> travelCities { get; set; }

        public DbSet<EventType> EventType { get; set; }

        public DbSet<Event> Event { get; set; }

        public DbSet<EventSpeaker> EventSpeaker { get; set; }

        public DbSet<EventTravelRequest> EventTravelRequest { get; set; }

        public DbSet<AccountSupportiveVisit> AccountSupportiveVisit { get; set; }

        public DbSet<AccountSupportiveVisitProduct> AccountSupportiveVisitproduct { get; set; }

        public DbSet<AccountSupportiveVisitPerson> AccountSupportiveVisitPerson { get; set; }

        public DbSet<RequestDeleteAccountSupportive> RequestDeleteAccountSupportive { get; set; }

        public DbSet<EventProposalRequest> EventProposalRequest { get; set; }

        public DbSet<AccountSupportiveVisitChat> AccountSupportiveVisitChat { get; set; }

        public DbSet<UserSubmanager> UserSubmanager { get; set; }

        public DbSet<UserVisitsLimit> usersVisitsLimit { get; set; }

        public DbSet<Passwords> passwords { get; set; }

        public DbSet<TED> ted { get; set; }

        public DbSet<Saam> saam { get; set; }

        public DbSet<Invited> invited { get; set; }

        public DbSet<SaudiDerm> saudiDerm { get; set; }

        public DbSet<JeddahDerm> jeddahDerm { get; set; }

        public DbSet<JeddaDermBoth> jeddaDermBoth { get; set; }

        public DbSet<MagellanDayAttendance> magellanDayAttendance { get; set; }

        public DbSet<AccountDevices> accountDevices { get; set; }

        public DbSet<MaintenanceRequest> maintenanceRequest { get; set; }

        public DbSet<TrainingRequest> trainingRequest { get; set; }

        public DbSet<PasswordChange> passwordChange { get; set; }

        public DbSet<EventFees> eventFees { get; set; }

        public DbSet<EventFeesRequest> eventFeesRequest { get; set; }

        public DbSet<ImcasComment> imcasComment { get; set; }
    }
}
