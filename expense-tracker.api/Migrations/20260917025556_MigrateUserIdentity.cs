using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace expense_tracker.api.Migrations
{
    /// <inheritdoc />
    public partial class MigrateUserIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    // 1. Temporarily remove the existing User -> Expense relationship
    migrationBuilder.DropForeignKey(
        name: "FK_Expenses_Users_UserId",
        table: "Expenses");

    // 2. Create a temporary GUID for every existing user
    migrationBuilder.AddColumn<Guid>(
        name: "NewId",
        table: "Users",
        type: "uuid",
        nullable: false,
        defaultValueSql: "gen_random_uuid()");

    // 3. Create a temporary GUID column for the Expense -> User relationship
    migrationBuilder.AddColumn<Guid>(
        name: "NewUserId",
        table: "Expenses",
        type: "uuid",
        nullable: true);

    // 4. Map each expense to its user's new GUID
    migrationBuilder.Sql("""
                         UPDATE "Expenses" AS e
                         SET "NewUserId" = u."NewId"
                         FROM "Users" AS u
                         WHERE e."UserId" = u."Id";
                         """);

    // 5. Verify that every expense received a new User GUID
    migrationBuilder.Sql("""
                         DO $$
                         BEGIN
                             IF EXISTS (
                                 SELECT 1
                                 FROM "Expenses"
                                 WHERE "NewUserId" IS NULL
                             ) THEN
                                 RAISE EXCEPTION
                                     'User identity migration failed: one or more expenses have no mapped user.';
                         END IF;
                         END $$;
                         """);

    // 6. Remove the old primary key
    migrationBuilder.DropPrimaryKey(
        name: "PK_Users",
        table: "Users");

    // 7. Remove the old integer UserId from Expenses
    migrationBuilder.DropColumn(
        name: "UserId",
        table: "Expenses");

    // 8. Remove the old integer User Id
    migrationBuilder.DropColumn(
        name: "Id",
        table: "Users");

    // 9. Rename the temporary GUID columns to their final names
    migrationBuilder.RenameColumn(
        name: "NewId",
        table: "Users",
        newName: "Id");

    migrationBuilder.RenameColumn(
        name: "NewUserId",
        table: "Expenses",
        newName: "UserId");

    // 10. Make the new User GUID the primary key
    migrationBuilder.AddPrimaryKey(
        name: "PK_Users",
        table: "Users",
        column: "Id");

    // 11. Make the new UserId the foreign key
    migrationBuilder.AddForeignKey(
        name: "FK_Expenses_Users_UserId",
        table: "Expenses",
        column: "UserId",
        principalTable: "Users",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);

    // 12. Recreate the index on Expense.UserId
    migrationBuilder.CreateIndex(
        name: "IX_Expenses_UserId",
        table: "Expenses",
        column: "UserId");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "Reverting this migration is not supported because the original integer User IDs are not preserved.");
        }
    }
}
