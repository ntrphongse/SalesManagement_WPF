using BusinessObject;
using DataAccess.DAO;
using System.Collections.Generic;

namespace DataAccess.Repository
{
    public class MemberRepository : IMemberRepository
    {
        public Member Add(Member newMember) => MemberDAO.Instance.Add(newMember);

        public void Delete(int memberId) => MemberDAO.Instance.Delete(memberId);

        public Member GetMember(string memberEmail) => MemberDAO.Instance.GetMember(memberEmail);

        public Member GetMember(int memberId) => MemberDAO.Instance.GetMember(memberId);

        public IEnumerable<Member> GetMembers() => MemberDAO.Instance.GetMembers();

        public int GetNextMemberId() => MemberDAO.Instance.GetNextMemberId();

        public Member Login(string email, string password) => MemberDAO.Instance.Login(email, password);

        public Member Update(Member updatedMember) => MemberDAO.Instance.Update(updatedMember);
    }
}
