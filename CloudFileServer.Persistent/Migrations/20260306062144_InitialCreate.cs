using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CloudFileServer.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NodeTypes",
                columns: table => new
                {
                    Id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<short>(type: "smallint", nullable: false),
                    IsLeaf = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NodeTypeId = table.Column<short>(type: "smallint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    StoragePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nodes_NodeTypes_NodeTypeId",
                        column: x => x.NodeTypeId,
                        principalTable: "NodeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Nodes_Nodes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NodeImageMeta",
                columns: table => new
                {
                    NodeId = table.Column<long>(type: "bigint", nullable: false),
                    WidthPx = table.Column<int>(type: "int", nullable: false),
                    HeightPx = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeImageMeta", x => x.NodeId);
                    table.ForeignKey(
                        name: "FK_NodeImageMeta_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NodeTags",
                columns: table => new
                {
                    NodeId = table.Column<long>(type: "bigint", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeTags", x => new { x.NodeId, x.TagId });
                    table.ForeignKey(
                        name: "FK_NodeTags_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NodeTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NodeTextMeta",
                columns: table => new
                {
                    NodeId = table.Column<long>(type: "bigint", nullable: false),
                    Encoding = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeTextMeta", x => x.NodeId);
                    table.ForeignKey(
                        name: "FK_NodeTextMeta_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NodeWordMeta",
                columns: table => new
                {
                    NodeId = table.Column<long>(type: "bigint", nullable: false),
                    PageCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeWordMeta", x => x.NodeId);
                    table.ForeignKey(
                        name: "FK_NodeWordMeta_Nodes_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Nodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "NodeTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "IsLeaf" },
                values: new object[,]
                {
                    { (short)1, (short)0, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), false },
                    { (short)2, (short)1, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), true },
                    { (short)3, (short)2, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), true },
                    { (short)4, (short)3, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), true }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Color", "Name" },
                values: new object[,]
                {
                    { 1, "#F44336", "Urgent" },
                    { 2, "#2196F3", "Work" },
                    { 3, "#4CAF50", "Personal" }
                });

            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IsDeleted", "Name", "NodeTypeId", "ParentId", "SizeBytes", "StoragePath", "UpdatedAt" },
                values: new object[,]
                {
                    { 1L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Root", (short)1, null, null, null, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Documents", (short)1, 1L, null, null, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Images", (short)1, 1L, null, null, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Personal", (short)1, 1L, null, null, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Annual Report", (short)2, 2L, 153600L, "docs/annual-report.docx", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Meeting Notes", (short)4, 2L, 2048L, "docs/meeting-notes.txt", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Sunset", (short)3, 3L, 512000L, "images/sunset.jpg", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Profile", (short)3, 3L, 204800L, "images/profile.jpg", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9L, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Diary", (short)2, 4L, 81920L, "personal/diary.docx", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "NodeTags",
                columns: new[] { "NodeId", "TagId" },
                values: new object[,]
                {
                    { 5L, 1 },
                    { 5L, 2 },
                    { 6L, 2 },
                    { 8L, 3 },
                    { 9L, 3 }
                });

            migrationBuilder.InsertData(
                table: "NodeWordMeta",
                columns: new[] { "NodeId", "PageCount" },
                values: new object[,]
                {
                    { 5L, 12 },
                    { 9L, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_NodeTypeId",
                table: "Nodes",
                column: "NodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_ParentId",
                table: "Nodes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_NodeTags_TagId",
                table: "NodeTags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeImageMeta");

            migrationBuilder.DropTable(
                name: "NodeTags");

            migrationBuilder.DropTable(
                name: "NodeTextMeta");

            migrationBuilder.DropTable(
                name: "NodeWordMeta");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Nodes");

            migrationBuilder.DropTable(
                name: "NodeTypes");
        }
    }
}
