import { cookies } from 'next/headers';
import Link from 'next/link';
import { 
  LayoutDashboard, 
  ShieldCheck, 
  FileText, 
  Settings, 
  LogOut, 
  Activity, 
  Bell 
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';

export default async function Sidebar() {
  const cookieStore = await cookies();
  const role = cookieStore.get('userRole')?.value;
  const userInfoStr = cookieStore.get('userInfo')?.value;
  const userInfo = userInfoStr ? JSON.parse(userInfoStr) : { firstName: "Unknown", lastName: "User" };

  const navItems = [
    { name: 'Dashboard', icon: LayoutDashboard, href: '/dashboard/employees', show: true },
    { name: 'Incidents', icon: ShieldCheck, href: '/dashboard/incidents', show: true },
    { name: 'Compliance', icon: FileText, href: '/dashboard/compliance', show: role === 'ComplianceOfficer' },
    { name: 'Reports', icon: FileText, href: '/dashboard/reports', show: true },
    { name: 'Services', icon: Activity, href: '/dashboard/services', show: true },
  ];

  return (
    <aside className="w-64 bg-[#09090b] border-r border-zinc-800 flex flex-col shrink-0">
      {/* Brand Logo */}
      <div className="p-6 flex items-center gap-3">
        <div className="w-8 h-8 bg-blue-600 rounded-md flex items-center justify-center shadow-lg shadow-blue-500/20">
          <ShieldCheck className="w-5 h-5 text-white" />
        </div>
        <span className="font-bold text-lg tracking-tight text-zinc-100">CrisisCMD</span>
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-4 space-y-1">
        {navItems.filter(i => i.show).map((item) => (
          <Link 
            key={item.name} 
            href={item.href} 
            className="flex items-center gap-3 px-3 py-2 text-sm font-medium text-zinc-400 hover:text-zinc-100 hover:bg-zinc-900 rounded-lg transition-all group"
          >
            <item.icon className="w-4 h-4 group-hover:text-blue-500" />
            {item.name}
          </Link>
        ))}
      </nav>

      {/* Bottom Actions */}
      <div className="px-4 space-y-1 mb-4">
        <Link 
          href="/dashboard/settings" 
          className="flex items-center gap-3 px-3 py-2 text-sm font-medium text-zinc-400 hover:text-zinc-100 hover:bg-zinc-900 rounded-lg transition-all"
        >
          <Settings className="w-4 h-4" />
          Settings
        </Link>
        <form action="/api/logout" method="POST">
          <button 
            type="submit"
            className="w-full flex items-center gap-3 px-3 py-2 text-sm font-medium text-red-400 hover:text-red-300 hover:bg-red-500/10 rounded-lg transition-all"
          >
            <LogOut className="w-4 h-4" />
            Log Out
          </button>
        </form>
      </div>

      {/* Profile Section */}
      <div className="p-4 border-t border-zinc-800 bg-zinc-950/50">
        <div className="flex items-center gap-3">
          <Avatar className="h-9 w-9 border border-zinc-700">
            <AvatarImage src="" /> {/* Add dynamic image if available */}
            <AvatarFallback className="bg-zinc-800 text-zinc-400 text-xs">
              {userInfo.firstName[0]}{userInfo.lastName[0]}
            </AvatarFallback>
          </Avatar>
          <div className="overflow-hidden">
            <p className="text-sm font-semibold text-zinc-200 truncate">
              {userInfo.firstName} {userInfo.lastName}
            </p>
            <p className="text-xs text-zinc-500 truncate capitalize">
              {role?.replace(/([A-Z])/g, ' $1').trim() || "User"}
            </p>
          </div>
        </div>
      </div>
    </aside>
  );
}