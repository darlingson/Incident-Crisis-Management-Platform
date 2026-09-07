'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { 
  ArrowRight, Edit3, AlertCircle, Clock, MapPin, 
  User, Shield, CheckCircle, ChevronRight, Paperclip
} from 'lucide-react';

const StatusMap: Record<string, number> = {
  "Reported": 0, "Acknowledged": 1, "UnderInvestigation": 2,
  "Mitigation": 3, "Resolved": 4, "PostIncidentReview": 5, "Closed": 6
};

export default function IncidentDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [data, setData] = useState<any>(null);
  const [isUpdating, setIsUpdating] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    const res = await fetch(`/api/reports/${id}`);
    const json = await res.json();
    setData(json);
  };

  useEffect(() => { if (id) fetchData(); }, [id]);

  const handleStatusUpdate = async (nextStatusName: string) => {
    setIsUpdating(true);
    setError(null);
    const res = await fetch(`/api/reports/${id}/status`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ 
        newStatus: StatusMap[nextStatusName],
        transitionNotes: `System: Incident progressed to ${nextStatusName}` 
      }),
    });

    const result = await res.json();
    if (!res.ok) {
      setError(result.error);
      setIsUpdating(false);
    } else {
      fetchData();
      setIsUpdating(false);
    }
  };

  if (!data) return <div className="bg-background min-h-screen animate-pulse" />;

  const nextAction = {
    "Reported": "Acknowledged",
    "Acknowledged": "UnderInvestigation",
    "UnderInvestigation": "Mitigation",
    "Mitigation": "Resolved",
    "Resolved": "PostIncidentReview",
    "PostIncidentReview": "Closed"
  }[data.status as string];

  return (
    <div className="bg-background min-h-screen text-foreground">
      {/* Top Header Bar */}
      <div className="border-b border-border bg-card/80 backdrop-blur-sm sticky top-0 z-10 px-6 py-4">
        <div className="flex justify-between items-center max-w-[1600px] mx-auto">
          <div className="flex items-center gap-4">
            <span className="text-muted-foreground font-mono text-sm">INC-{data.id}</span>
            <h1 className="text-xl font-semibold">{data.title}</h1>
            <span className={`px-2 py-0.5 rounded text-xs font-medium border ${
                data.impact === 'Critical' ? 'bg-destructive/10 text-destructive border-destructive/20' : 'bg-severity-medium/10 text-severity-medium border-severity-medium/20'
            }`}>
              {data.impact} Impact
            </span>
          </div>
          <div className="flex gap-3">
             <button onClick={() => router.push(`/dashboard/incidents/${id}/edit`)} className="flex items-center gap-2 px-4 py-2 bg-card border border-border rounded-lg text-sm font-medium hover:bg-muted transition-colors">
               <Edit3 className="w-4 h-4"/> Edit Details
             </button>
             {nextAction && (
               <button 
                 disabled={isUpdating}
                 onClick={() => handleStatusUpdate(nextAction)}
                 className="flex items-center gap-2 px-4 py-2 bg-primary rounded-lg text-sm font-medium text-primary-foreground hover:bg-primary/90 transition-colors"
               >
                 {isUpdating ? 'Processing...' : `Move to ${nextAction}`} <ArrowRight className="w-4 h-4"/>
               </button>
             )}
          </div>
        </div>
      </div>

      <div className="max-w-[1600px] mx-auto p-6 grid grid-cols-12 gap-6">
        {/* Left Col: Info Card */}
        <aside className="col-span-12 lg:col-span-3 space-y-6">
          <div className="bg-card border border-border rounded-lg p-5 shadow-sm">
            <h3 className="text-xs font-medium text-muted-foreground mb-6">Incident Details</h3>
            <div className="space-y-6">
                <DetailRow icon={<User className="w-4 h-4"/>} label="Owner" value={data.assignedTo || "Unassigned"} />
                <DetailRow icon={<MapPin className="w-4 h-4"/>} label="Location" value={data.location} />
                <DetailRow icon={<Clock className="w-4 h-4"/>} label="Started" value={new Date(data.createdAt).toLocaleString()} />
                <DetailRow icon={<Shield className="w-4 h-4"/>} label="Type" value={data.type} />
            </div>
          </div>
        </aside>

        {/* Center Col: Timeline & Activity */}
        <main className="col-span-12 lg:col-span-6 space-y-6">
          {error && (
            <div className="bg-destructive/10 border border-destructive/20 text-destructive p-4 rounded-lg flex items-center gap-3">
              <AlertCircle className="w-4 h-4" /> <span className="text-sm font-medium">{error}</span>
            </div>
          )}

          <div className="relative pl-8 border-l border-border space-y-8 ml-4">
            {data.history.length > 0 ? data.history.map((item: any, i: number) => (
              <div key={i} className="relative group">
                <div className="absolute -left-[41px] top-1 w-4 h-4 rounded-full bg-primary border-4 border-background group-hover:scale-110 transition-transform" />
                <div className="flex items-center gap-2 text-xs text-muted-foreground mb-2">
                  <span className="font-medium text-foreground">System</span>
                  <ChevronRight className="w-3 h-3"/>
                  <span>{new Date(item.changedAt).toLocaleTimeString()}</span>
                </div>
                <div className="bg-card border border-border p-4 rounded-lg shadow-sm">
                  <p className="text-sm font-medium">Status Transition</p>
                  <p className="text-sm text-muted-foreground mt-1">Moved from <span className="text-foreground">{item.oldStatus}</span> to <span className="text-primary font-medium">{item.newStatus}</span></p>
                  {item.transitionNotes && <p className="mt-3 text-xs text-muted-foreground italic border-t border-border pt-2">{item.transitionNotes}</p>}
                </div>
              </div>
            )) : (
              <div className="text-center py-10 text-muted-foreground text-sm italic">No status history records found.</div>
            )}
          </div>
        </main>

        {/* Right Col: Evidence & Resources */}
        <aside className="col-span-12 lg:col-span-3 space-y-6">
          <div className="bg-card border border-border rounded-lg p-5">
            <h3 className="text-xs font-medium text-muted-foreground mb-4">Evidence Files</h3>
            <div className="space-y-3">
              {data.evidenceFiles.map((f: string, i: number) => (
                <div key={i} className="p-3 bg-muted/30 border border-border rounded-lg flex items-center gap-3 group cursor-pointer hover:border-primary/20 transition-colors">
                  <div className="p-2 bg-primary/10 text-primary rounded-md">
                    <Paperclip className="w-4 h-4" />
                  </div>
                  <span className="text-xs truncate text-muted-foreground group-hover:text-foreground">{f}</span>
                </div>
              ))}
            </div>
          </div>
        </aside>
      </div>
    </div>
  );
}

function DetailRow({ icon, label, value }: any) {
  return (
    <div className="flex items-start gap-3">
      <div className="text-muted-foreground mt-0.5">{icon}</div>
      <div>
        <p className="text-xs text-muted-foreground mb-1">{label}</p>
        <p className="text-sm font-medium">{value}</p>
      </div>
    </div>
  );
}