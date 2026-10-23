using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AUBE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    T1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    T1Name = table.Column<string>(type: "TEXT", nullable: false),
                    T2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    T2Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Iterations = table.Column<int>(type: "INTEGER", nullable: false),
                    T1Wins = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalRoundsToWin = table.Column<int>(type: "INTEGER", nullable: false),
                    T2Wins = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalRoundsToWin = table.Column<int>(type: "INTEGER", nullable: false),
                    Draws = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalHealthLostOnWIn = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalHealthLostOnWIn = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalAttacks = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalDodgedAttacks = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalAttacks = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalDodgedAttacks = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalHits = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalBlockedHits = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalHits = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalBlockedHits = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalCriticalHits = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalCriticalHits = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalDamageDealt = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalDamageDealt = table.Column<int>(type: "INTEGER", nullable: false),
                    T1TotalArmorApplied = table.Column<int>(type: "INTEGER", nullable: false),
                    T2TotalArmorApplied = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ruleset = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Damage = table.Column<int>(type: "INTEGER", nullable: false),
                    NumberOfAttacks = table.Column<int>(type: "INTEGER", nullable: false),
                    AttackModifier = table.Column<int>(type: "INTEGER", nullable: false),
                    HitModifier = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ruleset = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    MaxHealth = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentHealth = table.Column<int>(type: "INTEGER", nullable: false),
                    Armor = table.Column<int>(type: "INTEGER", nullable: false),
                    AttackSkill = table.Column<int>(type: "INTEGER", nullable: false),
                    DodgeChance = table.Column<int>(type: "INTEGER", nullable: false),
                    BlockChance = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Units_WeaponId",
                table: "Units",
                column: "WeaponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Weapons");
        }
    }
}
