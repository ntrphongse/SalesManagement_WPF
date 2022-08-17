using AppSettings;
using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace DataAccess.DAO
{
    internal class MemberDAO
    {
        private static MemberDAO instance = null;
        private static readonly object instanceLock = new object();
        private MemberDAO()
        {

        }

        internal static MemberDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new MemberDAO();
                    }
                    return instance;
                }
            }
        }

        internal Member GetDefaultMember()
        {
            Member defaultMember = null;
            try
            {
                defaultMember = JsonSerializer.Deserialize<Member>(Configuration.DefaultAccount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return defaultMember;
        }

        internal Member Login(string email, string password)
        {
            Member loginMember = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                List<Member> members = database.Members.ToList();
                members.Add(GetDefaultMember());
                loginMember = members.SingleOrDefault(member => member.Email.ToLower().Equals(email.ToLower())
                                        && member.Password.Equals(password));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return loginMember;
        }

        internal IEnumerable<Member> GetMembers()
        {
            List<Member> members = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                members = database.Members.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return members;
        }

        internal int GetNextMemberId()
        {
            int nextId = -1;
            try
            {
                var database = new FStoreDBAssignmentContext();
                Member currentMax = database.Members.OrderByDescending(member => member.MemberId).First();
                nextId = currentMax.MemberId + 1;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return nextId;
        }

        internal Member GetMember(int memberId)
        {
            Member member = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                member = database.Members.SingleOrDefault(member => member.MemberId == memberId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }

        internal Member GetMember(string memberEmail)
        {
            Member member = null;
            try
            {
                var database = new FStoreDBAssignmentContext();
                member = database.Members.SingleOrDefault(member => member.Email.ToLower().Equals(memberEmail.ToLower()));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }

        internal Member Add(Member newMember)
        {
            Member member = null;
            try
            {
                if (GetMember(newMember.MemberId) != null)
                {
                    throw new Exception($"Member with the ID {newMember.MemberId} is existed! Please check with the developer for more information");
                }
                if (GetMember(newMember.Email) != null)
                {
                    throw new Exception($"Member with the email {newMember.Email} is existed! Please try again with a different email");
                }
                var database = new FStoreDBAssignmentContext();
                database.Members.Add(newMember);
                database.SaveChanges();
                member = newMember;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }

        internal Member Update(Member updatedMember)
        {
            Member member = null;
            try
            {
                if (GetMember(updatedMember.MemberId) == null)
                {
                    throw new Exception($"Member with the ID {updatedMember.MemberId} is not existed! Please check with the developer for more information");
                }
                var database = new FStoreDBAssignmentContext();
                database.Members.Update(updatedMember);
                database.SaveChanges();
                member = updatedMember;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return member;
        }

        internal void Delete(int memberId)
        {
            try
            {
                if (GetMember(memberId) == null)
                {
                    throw new Exception($"Member with the ID {memberId} is not existed! Please check again...");
                }
                var database = new FStoreDBAssignmentContext();
                database.Members.Remove(GetMember(memberId));
                database.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
