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
    <aside className="w-64 bg-card border-r border-border flex flex-col shrink-0">
      {/* Brand Logo */}
      <div className="p-6 flex items-center gap-3 border-b border-border">
        <div className="w-8 h-8 bg-primary rounded-md flex items-center justify-center">
          <ShieldCheck className="w-4 h-4 text-primary-foreground" />
        </div>
        <span className="font-semibold text-lg tracking-tight">CrisisCMD</span>
      </div>

      {/* Navigation */}
      <nav className="flex-1 px-4 py-4 space-y-1">
        {navItems.filter(i => i.show).map((item) => (
          <Link 
            key={item.name} 
            href={item.href} 
            className="flex items-center gap-3 px-3 py-2 text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted rounded-lg transition-colors group"
          >
            <item.icon className="w-4 h-4 group-hover:text-primary" />
            {item.name}
          </Link>
        ))}
      </nav>

      {/* Bottom Actions */}
      <div className="px-4 space-y-1 mb-4">
        <Link 
          href="/dashboard/settings" 
          className="flex items-center gap-3 px-3 py-2 text-sm font-medium text-muted-foreground hover:text-foreground hover:bg-muted rounded-lg transition-colors"
        >
          <Settings className="w-4 h-4" />
          Settings
        </Link>
        <form action="/api/logout" method="POST">
          <button 
            type="submit"
            className="w-full flex items-center gap-3 px-3 py-2 text-sm font-medium text-destructive hover:bg-destructive/10 rounded-lg transition-colors"
          >
            <LogOut className="w-4 h-4" />
            Log Out
          </button>
        </form>
      </div>

      {/* Profile Section */}
      <div className="p-4 border-t border-border bg-muted/30">
        <div className="flex items-center gap-3">
          <Avatar className="h-9 w-9 border border-border">
            <AvatarImage src="" />
            <AvatarFallback className="bg-muted text-muted-foreground text-xs">
              {userInfo.firstName[0]}{userInfo.lastName[0]}
            </AvatarFallback>
          </Avatar>
          <div className="overflow-hidden">
            <p className="text-sm font-medium truncate">
              {userInfo.firstName} {userInfo.lastName}
            </p>
            <p className="text-xs text-muted-foreground truncate capitalize">
              {role?.replace(/([A-Z])/g, ' $1').trim() || "User"}
            </p>
          </div>
        </div>
      </div>
    </aside>
  );
}