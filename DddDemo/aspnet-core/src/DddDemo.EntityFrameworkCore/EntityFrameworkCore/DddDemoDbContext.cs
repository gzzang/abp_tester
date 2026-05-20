using DddDemo.Orders;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace DddDemo.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class DddDemoDbContext :
    AbpDbContext<DddDemoDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public DddDemoDbContext(DbContextOptions<DddDemoDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        builder.Entity<Order>(b =>
        {
            b.ToTable(DddDemoConsts.DbTablePrefix + "Orders", DddDemoConsts.DbSchema);
            b.Property(x => x.OrderNo).IsRequired().HasMaxLength(50);
            b.Property(x => x.TotalAmount).HasPrecision(18, 2);
            b.Property(x => x.Remark).HasMaxLength(1000);
            b.OwnsOne(x => x.ShippingAddress, address =>
            {
                address.Property(x => x.Province).HasMaxLength(50);
                address.Property(x => x.City).HasMaxLength(50);
                address.Property(x => x.District).HasMaxLength(50);
                address.Property(x => x.Detail).HasMaxLength(500);
                address.Property(x => x.ZipCode).HasMaxLength(10);
                address.Property(x => x.ContactName).HasMaxLength(50);
                address.Property(x => x.ContactPhone).HasMaxLength(20);
            });
            b.HasIndex(x => x.OrderNo).IsUnique();
            b.HasIndex(x => x.CustomerId);
            b.HasIndex(x => x.Status);
        });

        builder.Entity<OrderItem>(b =>
        {
            b.ToTable(DddDemoConsts.DbTablePrefix + "OrderItems", DddDemoConsts.DbSchema);
            b.Property(x => x.ProductName).IsRequired().HasMaxLength(200);
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.HasIndex(x => x.OrderId);
        });
    }
}
