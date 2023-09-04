using System;
using Core.Entity.Entities;
using Core.Entity.Procedures;
using Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public ApplicationContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Connection.Instance.GetConnectionString());
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --------- ENTITY ---------
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<ParticipatingProvince>().ToTable("ParticipatingProvince");
            modelBuilder.Entity<Region>().ToTable("Region");
            modelBuilder.Entity<Chapter>().ToTable("Chapter");
            modelBuilder.Entity<Introduce>().ToTable("Introduce");
            modelBuilder.Entity<FAQs>().ToTable("FAQs");
            modelBuilder.Entity<TermCondition>().ToTable("TermCondition");
            modelBuilder.Entity<RoleType>().ToTable("RoleType");
            modelBuilder.Entity<Business>().ToTable("Business");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<FieldOperations>().ToTable("FieldOperations");
            modelBuilder.Entity<CustomerRole>().ToTable("CustomerRole");
            modelBuilder.Entity<Profession>().ToTable("Profession");
            modelBuilder.Entity<Provinces>().ToTable("Provinces");
            modelBuilder.Entity<Districts>().ToTable("Districts");
            modelBuilder.Entity<Wards>().ToTable("Wards");
            modelBuilder.Entity<Status>().ToTable("Status");
            modelBuilder.Entity<Expense>().ToTable("Expense");
            modelBuilder.Entity<Transaction>().ToTable("Transaction");
            modelBuilder.Entity<StatusTransaction>().ToTable("StatusTransaction");
            modelBuilder.Entity<MembershipAction>().ToTable("MembershipAction");
            modelBuilder.Entity<LogAction>().ToTable("LogAction");
            modelBuilder.Entity<StatusOpportunity>().ToTable("StatusOpportunity");
            modelBuilder.Entity<Opportunity>().ToTable("Opportunity");
            modelBuilder.Entity<Thanks>().ToTable("Thanks");
            modelBuilder.Entity<StatusFaceToFaceAndGuests>().ToTable("StatusFaceToFaceAndGuests");
            modelBuilder.Entity<FaceToFace>().ToTable("FaceToFace");
            modelBuilder.Entity<Guests>().ToTable("Guests");
            modelBuilder.Entity<Event>().ToTable("Event");
            modelBuilder.Entity<CustomerEvent>().ToTable("CustomerEvent");
            modelBuilder.Entity<TransactionEvent>().ToTable("TransactionEvent");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<CustomerCourse>().ToTable("CustomerCourse");
            modelBuilder.Entity<Assess>().ToTable("Assess");
            modelBuilder.Entity<TransactionCourse>().ToTable("TransactionCourse");
            modelBuilder.Entity<CustomerLikeEvent>().ToTable("CustomerLikeEvent");
            modelBuilder.Entity<CustomerShareEvent>().ToTable("CustomerShareEvent");
            modelBuilder.Entity<CustomerLikeCourse>().ToTable("CustomerLikeCourse");
            modelBuilder.Entity<CustomerShareCourse>().ToTable("CustomerShareCourse");
            modelBuilder.Entity<TimeEvent>().ToTable("TimeEvent");
            modelBuilder.Entity<TimeCourse>().ToTable("TimeCourse");
            modelBuilder.Entity<Notify>().ToTable("Notify");
            modelBuilder.Entity<NotifyType>().ToTable("NotifyType");
            modelBuilder.Entity<Page>().ToTable("Page");
            modelBuilder.Entity<RolePage>().ToTable("RolePage");
            modelBuilder.Entity<AssessCustomer>().ToTable("AssessCustomer");
            modelBuilder.Entity<MeetingChapter>().ToTable("MeetingChapter");
            modelBuilder.Entity<MeetingChapterCheckIn>().ToTable("MeetingChapterCheckIn");
            modelBuilder.Entity<AbsenceMedical>().ToTable("AbsenceMedical");

            // --------- PROC -------------
            modelBuilder.Entity<Proc_Permission>().ToTable(Proc_Permission.ProcName);
            modelBuilder.Entity<Proc_PermissionDetail>().ToTable(Proc_PermissionDetail.ProcName);
            modelBuilder.Entity<Proc_GetListUser>().ToTable(Proc_GetListUser.ProcName);
            modelBuilder.Entity<Proc_GetListRegion>().ToTable(Proc_GetListRegion.ProcName);
            modelBuilder.Entity<Proc_GetListChapter>().ToTable(Proc_GetListChapter.ProcName);
            modelBuilder.Entity<Proc_GetListRegionWithProvince>().ToTable(Proc_GetListRegionWithProvince.ProcName);
            modelBuilder.Entity<Proc_GetListRole>().ToTable(Proc_GetListRole.ProcName);
            modelBuilder.Entity<Proc_GetListBusiness>().ToTable(Proc_GetListBusiness.ProcName);
            modelBuilder.Entity<Proc_GetDetailBusiness>().ToTable(Proc_GetDetailBusiness.ProcName);
            modelBuilder.Entity<Poc_GetProvinceRegionWithChapterId>().ToTable(Poc_GetProvinceRegionWithChapterId.ProcName);
            modelBuilder.Entity<Proc_GetListCustomerPending>().ToTable(Proc_GetListCustomerPending.ProcName);
            modelBuilder.Entity<Proc_GetListCustomerWaitingActive>().ToTable(Proc_GetListCustomerWaitingActive.ProcName);
            modelBuilder.Entity<Proc_GetChapterInformation>().ToTable(Proc_GetChapterInformation.ProcName);
            modelBuilder.Entity<Proc_GetListMemberChapter>().ToTable(Proc_GetListMemberChapter.ProcName);
            modelBuilder.Entity<Proc_GetDetailMemberChapter>().ToTable(Proc_GetDetailMemberChapter.ProcName);
            modelBuilder.Entity<Proc_GetListOpportunity>().ToTable(Proc_GetListOpportunity.ProcName);
            modelBuilder.Entity<Proc_GetListThanks>().ToTable(Proc_GetListThanks.ProcName);
            modelBuilder.Entity<Proc_GetListFaceToFace>().ToTable(Proc_GetListFaceToFace.ProcName);
            modelBuilder.Entity<Proc_GetListGuests>().ToTable(Proc_GetListGuests.ProcName);
            modelBuilder.Entity<Proc_GetListHistoryTransaction>().ToTable(Proc_GetListHistoryTransaction.ProcName);
            modelBuilder.Entity<Proc_GetListCustomerOutOfChapter>().ToTable(Proc_GetListCustomerOutOfChapter.ProcName);
            modelBuilder.Entity<Proc_ReviewCreateOpportunity>().ToTable(Proc_ReviewCreateOpportunity.ProcName);
            modelBuilder.Entity<Proc_GetOpportunityReceiver>().ToTable(Proc_GetOpportunityReceiver.ProcName);
            modelBuilder.Entity<Proc_GetCustomerProfile>().ToTable(Proc_GetCustomerProfile.ProcName);
            modelBuilder.Entity<Proc_GetListCustomerEvent>().ToTable(Proc_GetListCustomerEvent.ProcName);
            modelBuilder.Entity<Proc_GetListTransactionEvent>().ToTable(Proc_GetListTransactionEvent.ProcName);
            modelBuilder.Entity<Proc_GetListCustomerCourse>().ToTable(Proc_GetListCustomerCourse.ProcName);
            modelBuilder.Entity<Proc_GetListTransactionCourse>().ToTable(Proc_GetListTransactionCourse.ProcName);
            modelBuilder.Entity<Proc_GetListAssess>().ToTable(Proc_GetListAssess.ProcName);
            modelBuilder.Entity<Proc_GetListAssessMobile>().ToTable(Proc_GetListAssessMobile.ProcName);
            modelBuilder.Entity<Proc_GetOpportunityGive>().ToTable(Proc_GetOpportunityGive.ProcName);
            modelBuilder.Entity<Proc_GetListEventMobile>().ToTable(Proc_GetListEventMobile.ProcName);
            modelBuilder.Entity<Proc_GetListCourseMobile>().ToTable(Proc_GetListCourseMobile.ProcName);
            modelBuilder.Entity<Proc_GetListVideoMobile>().ToTable(Proc_GetListVideoMobile.ProcName);
            modelBuilder.Entity<Proc_GetListLikedEventAndCourse>().ToTable(Proc_GetListLikedEventAndCourse.ProcName);
            modelBuilder.Entity<Proc_CheckUniqueFieldOperationsChapter>().ToTable(Proc_CheckUniqueFieldOperationsChapter.ProcName);
            modelBuilder.Entity<Proc_GetListChapterMobile>().ToTable(Proc_GetListChapterMobile.ProcName);
            modelBuilder.Entity<Proc_GetListNotify>().ToTable(Proc_GetListNotify.ProcName);
            modelBuilder.Entity<Proc_GetIndicators>().ToTable(Proc_GetIndicators.ProcName);
            modelBuilder.Entity<Proc_GetChapterMemberCompany>().ToTable(Proc_GetChapterMemberCompany.ProcName);
            modelBuilder.Entity<Proc_GetClassifications>().ToTable(Proc_GetClassifications.ProcName);
            modelBuilder.Entity<Proc_GetClassificationsNotInChapter>().ToTable(Proc_GetClassificationsNotInChapter.ProcName);
            modelBuilder.Entity<Proc_GetAllMeetingChapterExpired>().ToTable(Proc_GetAllMeetingChapterExpired.ProcName);
            modelBuilder.Entity<Proc_GetMeetingChapterWithCustomerId>().ToTable(Proc_GetMeetingChapterWithCustomerId.ProcName);
            modelBuilder.Entity<Proc_GetListTopCustomer>().ToTable(Proc_GetListTopCustomer.ProcName);
            modelBuilder.Entity<Proc_GetListRecentEvent>().ToTable(Proc_GetListRecentEvent.ProcName);
            modelBuilder.Entity<Proc_GetMembershipDuesReportAllMember>().ToTable(Proc_GetMembershipDuesReportAllMember.ProcName);
            modelBuilder.Entity<Proc_GetMembershipDuesReportAllMemberExpired>().ToTable(Proc_GetMembershipDuesReportAllMemberExpired.ProcName);
            modelBuilder.Entity<Proc_GetMembershipDuesReportAllMemberLate>().ToTable(Proc_GetMembershipDuesReportAllMemberLate.ProcName);
            modelBuilder.Entity<Proc_GetMembershipDuesReportAllNewMember>().ToTable(Proc_GetMembershipDuesReportAllNewMember.ProcName);
            modelBuilder.Entity<Proc_GetKPIMobile>().ToTable(Proc_GetKPIMobile.ProcName);
            modelBuilder.Entity<Proc_GetListSearchHomeMobile>().ToTable(Proc_GetListSearchHomeMobile.ProcName);
            modelBuilder.Entity<Proc_GetCustomerInChapterWithChapterId>().ToTable(Proc_GetCustomerInChapterWithChapterId.ProcName);
            modelBuilder.Entity<Proc_GetTransactionCEUInKpiWeb>().ToTable(Proc_GetTransactionCEUInKpiWeb.ProcName);
            modelBuilder.Entity<Proc_GetListAssessCustomer>().ToTable(Proc_GetListAssessCustomer.ProcName);
        }

        internal object Entry<T>()
        {
            throw new NotImplementedException();
        }
    }
}
