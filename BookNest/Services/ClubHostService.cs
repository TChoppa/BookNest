using BookNest.Interfaces;
using BookNest.IServices;
using BookNest.Models;

namespace BookNest.Services
{
    public class ClubHostService : IClubHostService
    {
        private readonly IClubHostRepository _clubHostRepo;
        private readonly IUserRepository _userRepo;
        public ClubHostService(IClubHostRepository clubHostRepo, IUserRepository userRepository)
        {
            _clubHostRepo = clubHostRepo;
            _userRepo = userRepository;
        }

        public async Task CreateRoom(ClubHostRoom room, string username)
        {
            room.Name = username;
            room.ClubHostCode = Guid.NewGuid().ToString().Substring(0, 8);
            //room.isActive = DateTime.Now >= room.StartTime && DateTime.Now <= room.EndTime;
            await _clubHostRepo.CreateRoom(room);

            var user = await _userRepo.GetUserByUsername(username);

            var creatorMember = new ClubHostMember
            {
                RoomId = room.Id,
                Name = user.Firstname + " " + user.Lastname
            };
            await _clubHostRepo.AddClubHostMemeber(creatorMember);
        }
        public async Task<List<ClubHostRoom>> GetAllRooms()
        {
            return await _clubHostRepo.GetAllRooms();
        }

        public async Task<ClubHostMember> JoinMember(int roomId, string ParticipantName)
        {
            var member = new ClubHostMember
            {
                RoomId = roomId,
                Name = ParticipantName
            };
            await _clubHostRepo.AddClubHostMemeber(member);
            return member;
        }
        public async Task AddAttachment(ClubHostAttachment attachment)
        {                        
            await _clubHostRepo.AddAttachment(attachment);
        }

    }
}
