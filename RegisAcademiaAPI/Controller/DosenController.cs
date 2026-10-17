
using AcademicApi.Data;
using AcademicApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DosenController : ControllerBase
{
    private readonly AppDbContext _context;

    public DosenController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/dosen
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Dosen>>> GetAll()
    {
        var data = await _context.Dosen
            .AsNoTracking()
            .Select(d => new
            {
                d.DosenId,
                d.Nidn,
                d.NamaDosen,
                d.Email,
                d.Prodi
            })
            .ToListAsync();

        return Ok(data);
    }

    // GET: api/dosen/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Dosen>> GetById(int id)
    {
        var dosen = await _context.Dosen
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DosenId == id);

        if (dosen == null)
            return NotFound(new { message = "Data dosen tidak ditemukan." });

        return Ok(new
        {
            dosen.DosenId,
            dosen.Nidn,
            dosen.NamaDosen,
            dosen.Email,
            dosen.Prodi
        });
    }

    // POST: api/dosen
    [HttpPost]
    public async Task<IActionResult> Create(Dosen dosen)
    {
        var nidnExists = await _context.Dosen
            .AnyAsync(d => d.Nidn == dosen.Nidn);

        if (nidnExists)
            return Conflict(new { message = "NIDN sudah terdaftar." });

        dosen.DosenId = 0;
        _context.Dosen.Add(dosen);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = dosen.DosenId },
            new
            {
                dosen.DosenId,
                dosen.Nidn,
                dosen.NamaDosen,
                dosen.Email,
                dosen.Prodi
            });
    }

    // PUT: api/dosen/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Dosen input)
    {
        var dosen = await _context.Dosen.FindAsync(id);

        if (dosen == null)
            return NotFound(new { message = "Data dosen tidak ditemukan." });

        var nidnExists = await _context.Dosen
            .AnyAsync(d => d.Nidn == input.Nidn && d.DosenId != id);

        if (nidnExists)
            return Conflict(new { message = "NIDN sudah digunakan dosen lain." });

        dosen.Nidn = input.Nidn;
        dosen.NamaDosen = input.NamaDosen;
        dosen.Email = input.Email;
        dosen.Prodi = input.Prodi;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Data dosen berhasil diperbarui." });
    }

    // DELETE: api/dosen/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dosen = await _context.Dosen.FindAsync(id);

        if (dosen == null)
            return NotFound(new { message = "Data dosen tidak ditemukan." });

        var masihMembimbing = await _context.Mahasiswa
            .AnyAsync(m => m.DosenId == id);

        if (masihMembimbing)
            return Conflict(new
            {
                message = "Dosen tidak dapat dihapus karena masih menjadi dosen wali mahasiswa."
            });

        _context.Dosen.Remove(dosen);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Data dosen berhasil dihapus." });
    }
}
