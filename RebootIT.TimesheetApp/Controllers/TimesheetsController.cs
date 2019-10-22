﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RebootIT.TimesheetApp.Data;

namespace RebootIT.TimesheetApp.Controllers
{
    public class TimesheetsController : Controller
    {
        private readonly TimesheetDbContext _context;

        public TimesheetsController(TimesheetDbContext context)
        {
            _context = context;
        }

        // GET: Timesheets
        public async Task<IActionResult> Index()
        {
            var timesheetDbContext = _context.Timesheets.Include(t => t.Client).Include(t => t.Location).Include(t => t.Staff);
            return View(await timesheetDbContext.ToListAsync());
        }

        // GET: Timesheet for Specific Staff
        public async Task<IActionResult> StaffTimesheet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheets = await _context.Timesheets.Include(t => t.Client)
                                            .Include(t => t.Location)
                                            .Include(t => t.Staff)
                                            .Where(t => t.StaffId == id)
                                            .ToListAsync();

            if (timesheets == null)
            {
                return NotFound();
            }

            ViewData["StaffId"] = id;

            return View("Index", timesheets);
        }

        //GET: Timesheets for Specific Client
        public async Task<IActionResult> ClientTimesheet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheets = await _context.Timesheets.Include(t => t.Client)
                                            .Include(t => t.Location)
                                            .Include(t => t.Staff)
                                            .Where(t => t.ClientId == id)
                                            .ToListAsync();

            if (timesheets == null)
            {
                return NotFound();
            }

            ViewData["ClientId"] = id;

            return View("Index", timesheets);
        }

        //GET: Timesheets for Specific Location
        public async Task<IActionResult> LocationTimesheet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheets = await _context.Timesheets.Include(t => t.Client)
                                            .Include(t => t.Location)
                                            .Include(t => t.Staff)
                                            .Where(t => t.LocationId == id)
                                            .ToListAsync();

            if (timesheets == null)
            {
                return NotFound();
            }

            ViewData["LocationId"] = id;

            return View("Index", timesheets);
        }

        // GET: Timesheets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets
                .Include(t => t.Client)
                .Include(t => t.Location)
                .Include(t => t.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // GET: Timesheets/Create
        public IActionResult Create(int? staffId, int? clientId, int? locationId)
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "CompanyName", clientId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", locationId);

            var staffList = _context.Staff.Select(ts => new { Id = ts.Id, FullName = ts.Forename + " " + ts.Surname  } );

            ViewData["StaffId"] = new SelectList(staffList, "Id", "FullName", staffId);
            
            return View();
        }

        // POST: Timesheets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MinutesWorked,StaffId,ClientId,LocationId")] Timesheet timesheet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timesheet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "CompanyName", timesheet.ClientId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", timesheet.LocationId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Email", timesheet.StaffId);
            return View(timesheet);
        }

        // GET: Timesheets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "CompanyName", timesheet.ClientId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", timesheet.LocationId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Email", timesheet.StaffId);
            return View(timesheet);
        }

        // POST: Timesheets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MinutesWorked,StaffId,ClientId,LocationId")] Timesheet timesheet)
        {
            if (id != timesheet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timesheet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimesheetExists(timesheet.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "CompanyName", timesheet.ClientId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", timesheet.LocationId);
            ViewData["StaffId"] = new SelectList(_context.Staff, "Id", "Email", timesheet.StaffId);
            return View(timesheet);
        }

        // GET: Timesheets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timesheet = await _context.Timesheets
                .Include(t => t.Client)
                .Include(t => t.Location)
                .Include(t => t.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timesheet == null)
            {
                return NotFound();
            }

            return View(timesheet);
        }

        // POST: Timesheets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            _context.Timesheets.Remove(timesheet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimesheetExists(int id)
        {
            return _context.Timesheets.Any(e => e.Id == id);
        }
    }
}
