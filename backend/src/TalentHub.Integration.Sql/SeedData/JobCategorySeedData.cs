namespace TalentHub.Integration.Sql.SeedData;

public static class JobCategorySeedData
{
    public static readonly object[] Rows =
    [
        new { Id = 1, Name = "Engineering", Slug = "engineering", Description = "Software engineering and development" },
        new { Id = 2, Name = "Product", Slug = "product", Description = "Product management and strategy" },
        new { Id = 3, Name = "Design", Slug = "design", Description = "UI, UX, and visual design" }
    ];
}
