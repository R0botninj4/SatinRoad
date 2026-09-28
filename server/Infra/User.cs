using LinqToDB.Mapping;

namespace Infra;

[Table("Users")]
public class User
{
    [PrimaryKey] public string Id { get; set; } = "";
    [Column, NotNull] public string Username { get; set; } = "";
    [Column, NotNull] public string NormalizedUsername { get; set; } = "";
    [Column, NotNull] public string PasswordHash { get; set; } = "";
}
