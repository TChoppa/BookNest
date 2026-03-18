using BookNest.Models;

namespace BookNest.IServices
{
    public interface IClubHostService       
    {
        public Task CreateRoom(ClubHostRoom room, string username);
        public Task<List<ClubHostRoom>> GetAllRooms();
        public  Task<ClubHostMember> JoinMember(int roomId, string ParticipantName);
        public  Task AddAttachment(ClubHostAttachment attachment);
    }
}
