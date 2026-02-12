use BankDB 
select 
  b.BranchName, 
  b.BranchAddress, 
  count(a.AccountID) as number_of_accounts 
from 
  Branches b 
  join Accounts a on b.BranchID = a.BranchID 
group by 
  b.BranchName, 
  b.BranchAddress 
having 
  count(a.AccountID) > 1
