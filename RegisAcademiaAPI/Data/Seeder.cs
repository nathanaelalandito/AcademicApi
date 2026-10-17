
using AcademicApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademicApi.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Dosen.AnyAsync())
            return;

        var dosen = new List<Dosen>
        {
            new()
            {
                Nidn = "0012345678",
                NamaDosen = "Dr. Andi Saputra",
                Email = "andi@kampus.ac.id",
                Prodi = "Informatika"
            },
            new()
            {
                Nidn = "0012345679",
                NamaDosen = "Siti Rahma, M.Kom.",
                Email = "siti@kampus.ac.id",
                Prodi = "Sistem Informasi"
            },
            new()
            {
                Nidn = "0012345680",
                NamaDosen = "Budi Santoso, M.T.",
                Email = "budi@kampus.ac.id",
                Prodi = "Informatika"
            }
        };

        context.Dosen.AddRange(dosen);
        await context.SaveChangesAsync();

        var mahasiswa = new List<Mahasiswa>
        {
            new()
            {
                Nim = "2301001",
                NamaMahasiswa = "Aditya Pratama",
                Email = "aditya@kampus.ac.id",
                Angkatan = 2023,
                DosenId = dosen[0].DosenId
            },
            new()
            {
                Nim = "2301002",
                NamaMahasiswa = "Nadia Putri",
                Email = "nadia@kampus.ac.id",
                Angkatan = 2023,
                DosenId = dosen[1].DosenId
            },
            new()
            {
                Nim = "2401001",
                NamaMahasiswa = "Rizky Firmansyah",
                Email = "rizky@kampus.ac.id",
                Angkatan = 2024,
                DosenId = dosen[0].DosenId
            },
            new()
            {
                Nim = "2401002",
                NamaMahasiswa = "Dewi Lestari",
                Email = "dewi@kampus.ac.id",
                Angkatan = 2024,
                DosenId = dosen[2].DosenId
            },
            new()
            {
                Nim = "2501001",
                NamaMahasiswa = "Fajar Ramadhan",
                Email = "fajar@kampus.ac.id",
                Angkatan = 2025,
                DosenId = dosen[1].DosenId
            }
        };

        context.Mahasiswa.AddRange(mahasiswa);
        await context.SaveChangesAsync();
    }
}
