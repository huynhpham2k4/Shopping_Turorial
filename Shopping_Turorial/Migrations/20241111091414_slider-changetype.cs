using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping_Tutorial.Migrations
{
	public partial class sliderchangetype : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
				name: "Image",
				table: "Sliders",
				type: "nvarchar(max)",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "int");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<int>(
				name: "Image",
				table: "Sliders",
				type: "int",
				nullable: false,
				defaultValue: 0,
				oldClrType: typeof(string),
				oldType: "nvarchar(max)",
				oldNullable: true);
		}
	}
}
