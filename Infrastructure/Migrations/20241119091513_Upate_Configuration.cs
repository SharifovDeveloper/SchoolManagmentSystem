using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Upate_Configuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_City_CityId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_Department_DepartmentId",
                table: "Student");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubject_Student_StudentId",
                table: "StudentSubject");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubject_Subject_SubjectId",
                table: "StudentSubject");

            migrationBuilder.DropForeignKey(
                name: "FK_Teacher_City_CityId",
                table: "Teacher");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubject_Subject_SubjectId",
                table: "TeacherSubject");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubject_Teacher_TeacherId",
                table: "TeacherSubject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teacher",
                table: "Teacher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subject",
                table: "Subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Student",
                table: "Student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.DropPrimaryKey(
                name: "PK_City",
                table: "City");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherSubject",
                table: "TeacherSubject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentSubject",
                table: "StudentSubject");

            migrationBuilder.RenameTable(
                name: "Teacher",
                newName: "teacher");

            migrationBuilder.RenameTable(
                name: "Subject",
                newName: "subject");

            migrationBuilder.RenameTable(
                name: "Student",
                newName: "student");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "department");

            migrationBuilder.RenameTable(
                name: "City",
                newName: "city");

            migrationBuilder.RenameTable(
                name: "TeacherSubject",
                newName: "teacher_subject");

            migrationBuilder.RenameTable(
                name: "StudentSubject",
                newName: "student_subject");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "teacher",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "teacher",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "teacher",
                newName: "last_updated_date");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "teacher",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "teacher",
                newName: "created_date");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "teacher",
                newName: "birth_date");

            migrationBuilder.RenameIndex(
                name: "IX_Teacher_CityId",
                table: "teacher",
                newName: "IX_teacher_CityId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "subject",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "subject",
                newName: "last_updated_date");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "subject",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "GradeLevel",
                table: "subject",
                newName: "grade_level");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "subject",
                newName: "created_date");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "student",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "student",
                newName: "gender");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "student",
                newName: "last_updated_date");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "student",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CurrentGradeLevel",
                table: "student",
                newName: "current_grade_level");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "student",
                newName: "created_date");

            migrationBuilder.RenameColumn(
                name: "BirthDate",
                table: "student",
                newName: "birth_date");

            migrationBuilder.RenameIndex(
                name: "IX_Student_DepartmentId",
                table: "student",
                newName: "IX_student_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Student_CityId",
                table: "student",
                newName: "IX_student_CityId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "department",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "department",
                newName: "last_updated_date");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "department",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "department",
                newName: "created_date");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "city",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "city",
                newName: "last_updated_date");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "city",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "city",
                newName: "created_date");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "teacher_subject",
                newName: "last_updated_date");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "teacher_subject",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "teacher_subject",
                newName: "created_date");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherSubject_TeacherId",
                table: "teacher_subject",
                newName: "IX_teacher_subject_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_TeacherSubject_SubjectId",
                table: "teacher_subject",
                newName: "IX_teacher_subject_SubjectId");

            migrationBuilder.RenameColumn(
                name: "Mark",
                table: "student_subject",
                newName: "mark");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedDate",
                table: "student_subject",
                newName: "last_updated_date");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "student_subject",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "student_subject",
                newName: "created_date");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubject_SubjectId",
                table: "student_subject",
                newName: "IX_student_subject_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubject_StudentId",
                table: "student_subject",
                newName: "IX_student_subject_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_teacher",
                table: "teacher",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subject",
                table: "subject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student",
                table: "student",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_department",
                table: "department",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_city",
                table: "city",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_teacher_subject",
                table: "teacher_subject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_student_subject",
                table: "student_subject",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_student_city",
                table: "student",
                column: "CityId",
                principalTable: "city",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_student_department",
                table: "student",
                column: "DepartmentId",
                principalTable: "department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_student_subject_student",
                table: "student_subject",
                column: "StudentId",
                principalTable: "student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_student_subject_subject",
                table: "student_subject",
                column: "SubjectId",
                principalTable: "subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_teacher_city",
                table: "teacher",
                column: "CityId",
                principalTable: "city",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_teacher_subject_subject",
                table: "teacher_subject",
                column: "SubjectId",
                principalTable: "subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_teacher_subject_teacher",
                table: "teacher_subject",
                column: "TeacherId",
                principalTable: "teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_student_city",
                table: "student");

            migrationBuilder.DropForeignKey(
                name: "fk_student_department",
                table: "student");

            migrationBuilder.DropForeignKey(
                name: "fk_student_subject_student",
                table: "student_subject");

            migrationBuilder.DropForeignKey(
                name: "fk_student_subject_subject",
                table: "student_subject");

            migrationBuilder.DropForeignKey(
                name: "fk_teacher_city",
                table: "teacher");

            migrationBuilder.DropForeignKey(
                name: "fk_teacher_subject_subject",
                table: "teacher_subject");

            migrationBuilder.DropForeignKey(
                name: "fk_teacher_subject_teacher",
                table: "teacher_subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_teacher",
                table: "teacher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subject",
                table: "subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student",
                table: "student");

            migrationBuilder.DropPrimaryKey(
                name: "PK_department",
                table: "department");

            migrationBuilder.DropPrimaryKey(
                name: "PK_city",
                table: "city");

            migrationBuilder.DropPrimaryKey(
                name: "PK_teacher_subject",
                table: "teacher_subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_student_subject",
                table: "student_subject");

            migrationBuilder.RenameTable(
                name: "teacher",
                newName: "Teacher");

            migrationBuilder.RenameTable(
                name: "subject",
                newName: "Subject");

            migrationBuilder.RenameTable(
                name: "student",
                newName: "Student");

            migrationBuilder.RenameTable(
                name: "department",
                newName: "Department");

            migrationBuilder.RenameTable(
                name: "city",
                newName: "City");

            migrationBuilder.RenameTable(
                name: "teacher_subject",
                newName: "TeacherSubject");

            migrationBuilder.RenameTable(
                name: "student_subject",
                newName: "StudentSubject");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Teacher",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Teacher",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "last_updated_date",
                table: "Teacher",
                newName: "LastUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Teacher",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "Teacher",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "birth_date",
                table: "Teacher",
                newName: "BirthDate");

            migrationBuilder.RenameIndex(
                name: "IX_teacher_CityId",
                table: "Teacher",
                newName: "IX_Teacher_CityId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Subject",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "last_updated_date",
                table: "Subject",
                newName: "LastUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Subject",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "grade_level",
                table: "Subject",
                newName: "GradeLevel");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "Subject",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Student",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Student",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "last_updated_date",
                table: "Student",
                newName: "LastUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Student",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "current_grade_level",
                table: "Student",
                newName: "CurrentGradeLevel");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "Student",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "birth_date",
                table: "Student",
                newName: "BirthDate");

            migrationBuilder.RenameIndex(
                name: "IX_student_DepartmentId",
                table: "Student",
                newName: "IX_Student_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_student_CityId",
                table: "Student",
                newName: "IX_Student_CityId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Department",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "last_updated_date",
                table: "Department",
                newName: "LastUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Department",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "Department",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "City",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "last_updated_date",
                table: "City",
                newName: "LastUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "City",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "City",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "last_updated_date",
                table: "TeacherSubject",
                newName: "LastUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "TeacherSubject",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "TeacherSubject",
                newName: "CreatedDate");

            migrationBuilder.RenameIndex(
                name: "IX_teacher_subject_TeacherId",
                table: "TeacherSubject",
                newName: "IX_TeacherSubject_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_teacher_subject_SubjectId",
                table: "TeacherSubject",
                newName: "IX_TeacherSubject_SubjectId");

            migrationBuilder.RenameColumn(
                name: "mark",
                table: "StudentSubject",
                newName: "Mark");

            migrationBuilder.RenameColumn(
                name: "last_updated_date",
                table: "StudentSubject",
                newName: "LastUpdatedDate");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "StudentSubject",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_date",
                table: "StudentSubject",
                newName: "CreatedDate");

            migrationBuilder.RenameIndex(
                name: "IX_student_subject_SubjectId",
                table: "StudentSubject",
                newName: "IX_StudentSubject_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_student_subject_StudentId",
                table: "StudentSubject",
                newName: "IX_StudentSubject_StudentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teacher",
                table: "Teacher",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subject",
                table: "Subject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Student",
                table: "Student",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_City",
                table: "City",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherSubject",
                table: "TeacherSubject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentSubject",
                table: "StudentSubject",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_City_CityId",
                table: "Student",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Department_DepartmentId",
                table: "Student",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubject_Student_StudentId",
                table: "StudentSubject",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubject_Subject_SubjectId",
                table: "StudentSubject",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teacher_City_CityId",
                table: "Teacher",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubject_Subject_SubjectId",
                table: "TeacherSubject",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubject_Teacher_TeacherId",
                table: "TeacherSubject",
                column: "TeacherId",
                principalTable: "Teacher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
