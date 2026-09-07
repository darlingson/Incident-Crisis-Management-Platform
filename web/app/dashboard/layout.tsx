import AppSidebar from "@/components/Sidebar";
import { SidebarProvider, SidebarInset, SidebarTrigger } from "@/components/ui/sidebar";
import { Separator } from "@/components/ui/separator";

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <SidebarProvider>
      <AppSidebar />
      <SidebarInset className="bg-muted/20">
        <header className="flex h-12 shrink-0 items-center gap-2 border-b border-border bg-background px-4">
          <SidebarTrigger className="-ml-1" />
          <Separator orientation="vertical" className="mr-2 h-4" />
        </header>
        <div className="flex flex-1 flex-col p-6 lg:p-8 max-w-7xl mx-auto w-full">
          {children}
        </div>
      </SidebarInset>
    </SidebarProvider>
  );
}