using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDM.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coproprietaires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coproprietaires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SousCoproprietes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SousCoproprietes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trimestres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Annee = table.Column<int>(type: "INTEGER", nullable: false),
                    Numero = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPrevisionnel = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalReel = table.Column<decimal>(type: "TEXT", nullable: false),
                    EstValide = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trimestres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroLot = table.Column<string>(type: "TEXT", nullable: false),
                    Tantiemes = table.Column<int>(type: "INTEGER", nullable: false),
                    CoproprietaireId = table.Column<int>(type: "INTEGER", nullable: false),
                    SousCoproprieteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lots_Coproprietaires_CoproprietaireId",
                        column: x => x.CoproprietaireId,
                        principalTable: "Coproprietaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lots_SousCoproprietes_SousCoproprieteId",
                        column: x => x.SousCoproprieteId,
                        principalTable: "SousCoproprietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppelsDeFonds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrimestreId = table.Column<int>(type: "INTEGER", nullable: false),
                    CoproprietaireId = table.Column<int>(type: "INTEGER", nullable: false),
                    MontantDu = table.Column<decimal>(type: "TEXT", nullable: false),
                    MontantRegle = table.Column<decimal>(type: "TEXT", nullable: false),
                    Regularisation = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppelsDeFonds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppelsDeFonds_Coproprietaires_CoproprietaireId",
                        column: x => x.CoproprietaireId,
                        principalTable: "Coproprietaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppelsDeFonds_Trimestres_TrimestreId",
                        column: x => x.TrimestreId,
                        principalTable: "Trimestres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargeTrimestres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Libelle = table.Column<string>(type: "TEXT", nullable: false),
                    MontantPrevisionnel = table.Column<decimal>(type: "TEXT", nullable: false),
                    MontantReel = table.Column<decimal>(type: "TEXT", nullable: false),
                    TrimestreId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeTrimestres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChargeTrimestres_Trimestres_TrimestreId",
                        column: x => x.TrimestreId,
                        principalTable: "Trimestres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargeTrimestreSousCopros",
                columns: table => new
                {
                    ChargeTrimestreId = table.Column<int>(type: "INTEGER", nullable: false),
                    SousCoproprieteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeTrimestreSousCopros", x => new { x.ChargeTrimestreId, x.SousCoproprieteId });
                    table.ForeignKey(
                        name: "FK_ChargeTrimestreSousCopros_ChargeTrimestres_ChargeTrimestreId",
                        column: x => x.ChargeTrimestreId,
                        principalTable: "ChargeTrimestres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChargeTrimestreSousCopros_SousCoproprietes_SousCoproprieteId",
                        column: x => x.SousCoproprieteId,
                        principalTable: "SousCoproprietes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppelsDeFonds_CoproprietaireId",
                table: "AppelsDeFonds",
                column: "CoproprietaireId");

            migrationBuilder.CreateIndex(
                name: "IX_AppelsDeFonds_TrimestreId",
                table: "AppelsDeFonds",
                column: "TrimestreId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeTrimestres_TrimestreId",
                table: "ChargeTrimestres",
                column: "TrimestreId");

            migrationBuilder.CreateIndex(
                name: "IX_ChargeTrimestreSousCopros_SousCoproprieteId",
                table: "ChargeTrimestreSousCopros",
                column: "SousCoproprieteId");

            migrationBuilder.CreateIndex(
                name: "IX_Coproprietaires_Nom",
                table: "Coproprietaires",
                column: "Nom");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_CoproprietaireId",
                table: "Lots",
                column: "CoproprietaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_NumeroLot",
                table: "Lots",
                column: "NumeroLot",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lots_SousCoproprieteId",
                table: "Lots",
                column: "SousCoproprieteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppelsDeFonds");

            migrationBuilder.DropTable(
                name: "ChargeTrimestreSousCopros");

            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "ChargeTrimestres");

            migrationBuilder.DropTable(
                name: "Coproprietaires");

            migrationBuilder.DropTable(
                name: "SousCoproprietes");

            migrationBuilder.DropTable(
                name: "Trimestres");
        }
    }
}
