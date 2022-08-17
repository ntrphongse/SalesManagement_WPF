using BusinessObject;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public interface IMemberRepository
    {
        public Member Login(string email, string password);
        public IEnumerable<Member> GetMembers();
        public int GetNextMemberId();
        public Member GetMember(string memberEmail);
        public Member GetMember(int memberId);
        public Member Add(Member newMember);
        public Member Update(Member updatedMember);
        public void Delete(int memberId);
    }
}
