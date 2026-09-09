import { cookies } from 'next/headers';
import Link from 'next/link';
import { 
  LayoutDashboard, 
  ShieldCheck, 
  FileText, 
  Settings, 
  LogOut, 
  Activity
} from 'lucide-react';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarSeparator,
} from '@/components/ui/sidebar';

export default async function AppSidebar() {
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
    <Sidebar variant="sidebar" collapsible="offcanvas">
      <SidebarHeader className="border-b border-border">
        <div className="flex items-center gap-3 px-2 py-2">
          <div className="w-8 h-8 bg-primary rounded-md flex items-center justify-center">
            <ShieldCheck className="w-4 h-4 text-primary-foreground" />
          </div>
          <span className="font-semibold text-lg tracking-tight">IncidentDesk</span>
        </div>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu>
              {navItems.filter(i => i.show).map((item) => (
                <SidebarMenuItem key={item.name}>
                  <SidebarMenuButton asChild tooltip={item.name}>
                    <Link href={item.href}>
                      <item.icon />
                      <span>{item.name}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton asChild>
              <Link href="/dashboard/settings">
                <Settings />
                <span>Settings</span>
              </Link>
            </SidebarMenuButton>
          </SidebarMenuItem>
          <SidebarMenuItem>
            <form action="/api/logout" method="POST" className="w-full">
              <SidebarMenuButton asChild className="text-destructive hover:bg-destructive/10 hover:text-destructive">
                <button type="submit" className="w-full">
                  <LogOut />
                  <span>Log Out</span>
                </button>
              </SidebarMenuButton>
            </form>
          </SidebarMenuItem>
        </SidebarMenu>
        <SidebarSeparator />
        <div className="flex items-center gap-3 px-2 py-2">
          <Avatar className="h-8 w-8 border border-border">
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
      </SidebarFooter>
    </Sidebar>
  );
}