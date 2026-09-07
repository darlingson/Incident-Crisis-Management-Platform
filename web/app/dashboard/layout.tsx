import Sidebar from "@/components/Sidebar";
import { ScrollArea } from "@/components/ui/scroll-area";

export default function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="flex h-screen overflow-hidden bg-background">
      <Sidebar />
      <main className="flex-1 flex flex-col min-w-0 bg-muted/20">
        <ScrollArea className="h-full w-full">
          <div className="container p-6 lg:p-8 max-w-7xl mx-auto">
            {children}
          </div>
        </ScrollArea>
      </main>
    </div>
  );
}