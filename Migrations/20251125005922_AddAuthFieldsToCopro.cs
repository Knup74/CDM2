using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDM.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthFieldsToCopro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Coproprietaires",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Coproprietaires",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Coproprietaires",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Coproprietaires",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Coproprietaires",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Coproprietaires");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Coproprietaires");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Coproprietaires");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Coproprietaires");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Coproprietaires",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
