
using AcademicApi.Data;
using AcademicApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MahasiswaController : ControllerBase
{
    private readonly AppDbContext _context;

    public MahasiswaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Mahasiswa
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _context.Mahasiswa
            .AsNoTracking()
            .Select(m => new
            {
                m.MahasiswaId,
                m.Nim,
                m.NamaMahasiswa,
                m.Email,
                m.Angkatan,
                m.DosenId,
                NamaDosenWali = m.DosenWali != null
                    ? m.DosenWali.NamaDosen
                    : ""
            })
            .ToListAsync();

        return Ok(data);
    }

    // GET: api/Mahasiswa/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var m = await _context.Mahasiswa
            .AsNoTracking()
            .Where(x => x.MahasiswaId == id)
            .Select(x => new
            {
                x.MahasiswaId,
                x.Nim,
                x.NamaMahasiswa,
                x.Email,
                x.Angkatan,
                x.DosenId,
                NamaDosenWali = x.DosenWali != null
                    ? x.DosenWali.NamaDosen
                    : ""
            })
            .FirstOrDefaultAsync();

        if (m == null)
            return NotFound(new { message = "Mahasiswa tidak ditemukan." });

        return Ok(m);
    }

    // POST: api/Mahasiswa
    [HttpPost]
    public async Task<IActionResult> Create(Mahasiswa input)
    {
        if (await _context.Mahasiswa.AnyAsync(m => m.Nim == input.Nim))
            return Conflict(new { message = "NIM sudah terdaftar." });

        if (!await _context.Dosen.AnyAsync(d => d.DosenId == input.DosenId))
            return BadRequest(new { message = "Dosen wali tidak ditemukan." });

        input.MahasiswaId = 0;
        _context.Mahasiswa.Add(input);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = input.MahasiswaId },
            new
            {
                input.MahasiswaId,
                input.Nim,
                input.NamaMahasiswa,
                input.Email,
                input.Angkatan,
                input.DosenId
            });
    }

    // PUT: api/Mahasiswa/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Mahasiswa input)
    {
        var mahasiswa = await _context.Mahasiswa.FindAsync(id);

        if (mahasiswa == null)
            return NotFound(new { message = "Mahasiswa tidak ditemukan." });

        if (await _context.Mahasiswa.AnyAsync(
            m => m.Nim == input.Nim && m.MahasiswaId != id))
        {
            return Conflict(new { message = "NIM sudah digunakan mahasiswa lain." });
        }

        if (!await _context.Dosen.AnyAsync(d => d.DosenId == input.DosenId))
            return BadRequest(new { message = "Dosen wali tidak ditemukan." });

        mahasiswa.Nim = input.Nim;
        mahasiswa.NamaMahasiswa = input.NamaMahasiswa;
        mahasiswa.Email = input.Email;
        mahasiswa.Angkatan = input.Angkatan;
        mahasiswa.DosenId = input.DosenId;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Data mahasiswa berhasil diperbarui." });
    }

    // DELETE: api/Mahasiswa/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var mahasiswa = await _context.Mahasiswa.FindAsync(id);

        if (mahasiswa == null)
            return NotFound(new { message = "Mahasiswa tidak ditemukan." });

        _context.Mahasiswa.Remove(mahasiswa);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Data mahasiswa berhasil dihapus." });
    }
}
