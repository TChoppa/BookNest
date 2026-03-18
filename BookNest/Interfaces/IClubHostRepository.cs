using BookNest.Models;

namespace BookNest.Interfaces
{
    public interface IClubHostRepository
    {
        public Task CreateRoom(ClubHostRoom room);
        public Task AddClubHostMemeber(ClubHostMember member);
        public Task<List<ClubHostRoom>> GetAllRooms();
        public Task AddMember(ClubHostMember member);
        public Task AddAttachment(ClubHostAttachment attachments);
    }

}
