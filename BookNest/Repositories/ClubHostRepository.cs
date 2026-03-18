using BookNest.DatabaseContext;
using BookNest.Enums;
using BookNest.Interfaces;
using BookNest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel;

namespace BookNest.Repositories
{
    public class ClubHostRepository :IClubHostRepository
    {
        //private readonly ClubHostRoom _clubHost;
        private readonly AppDbContext _dbContext;
        public ClubHostRepository(AppDbContext dbContext) {
            _dbContext = dbContext;
        
        }
        public async Task CreateRoom(ClubHostRoom room)
        {
             await _dbContext.ClubHostRooms.AddAsync(room);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddClubHostMemeber(ClubHostMember member)
        {
            await _dbContext.ClubHostMembers.AddAsync(member);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ClubHostRoom>> GetAllRooms()
        {
            var rooms = await _dbContext.ClubHostRooms.Include(r => r.Attachments)
            .Include(r => r.Members).ToListAsync();
            foreach (var room in rooms)
            {
                if(DateTime.Now >= room.StartTime && DateTime.Now <= room.EndTime)
                {
                    room.isActive = ClubHostEnum.Ongoing.ToString();
                }
                else if (DateTime.Now > room.EndTime)
                {
                    room.isActive = ClubHostEnum.Expired.ToString(); 
                }
                else
                {
                    room.isActive = ClubHostEnum.NotStarted.ToString(); ;
                }
            }
            await _dbContext.SaveChangesAsync();
            var orderedRooms = rooms
        .OrderByDescending(r => r.isActive == ClubHostEnum.Ongoing.ToString())
        .ThenByDescending(r => r.isActive == ClubHostEnum.NotStarted.ToString())
        .ThenBy(r => r.StartTime) // optional: sort within groups
        .ToList();
            return orderedRooms;
        }
        public async Task AddMember(ClubHostMember member)
        {
            await _dbContext.ClubHostMembers.AddAsync(member);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddAttachment(ClubHostAttachment attachments)
        {
            await _dbContext.ClubHostAttachments.AddAsync(attachments);
            await _dbContext.SaveChangesAsync();
        }

        //public async Task<int>GetRoomCount()
        //{
        //    return await _dbContext.ClubHostRooms.CountAsync();
        //}
    }
}
