'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { 
  ArrowRight, Edit3, AlertCircle, Clock, MapPin, 
  User, Shield, CheckCircle, ChevronRight 
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

  if (!data) return <div className="bg-[#0b0e14] min-h-screen animate-pulse" />;

  const nextAction = {
    "Reported": "Acknowledged",
    "Acknowledged": "UnderInvestigation",
    "UnderInvestigation": "Mitigation",
    "Mitigation": "Resolved",
    "Resolved": "PostIncidentReview",
    "PostIncidentReview": "Closed"
  }[data.status as string];

  return (
    <div className="bg-[#0b0e14] min-h-screen text-slate-300 font-sans">
      {/* Top Header Bar */}
      <div className="border-b border-slate-800 bg-[#0b0e14]/80 backdrop-blur-md sticky top-0 z-10 px-6 py-4">
        <div className="flex justify-between items-center max-w-[1600px] mx-auto">
          <div className="flex items-center gap-4">
            <span className="text-slate-500 font-mono text-sm">INC-{data.id}</span>
            <h1 className="text-xl font-bold text-white">{data.title}</h1>
            <span className={`px-2 py-0.5 rounded text-[10px] font-bold uppercase border ${
                data.impact === 'Critical' ? 'bg-red-900/20 text-red-500 border-red-800' : 'bg-orange-900/20 text-orange-500 border-orange-800'
            }`}>
              {data.impact} Impact
            </span>
          </div>
          <div className="flex gap-3">
             <button onClick={() => router.push(`/dashboard/incidents/${id}/edit`)} className="flex items-center gap-2 px-4 py-2 bg-[#1a1f29] border border-slate-700 rounded-lg text-sm font-bold hover:bg-slate-800 transition-all">
               <Edit3 size={14}/> Edit Details
             </button>
             {nextAction && (
               <button 
                 disabled={isUpdating}
                 onClick={() => handleStatusUpdate(nextAction)}
                 className="flex items-center gap-2 px-4 py-2 bg-blue-600 rounded-lg text-sm font-bold text-white hover:bg-blue-500 transition-all shadow-[0_0_15px_rgba(37,99,235,0.3)]"
               >
                 {isUpdating ? 'Processing...' : `Move to ${nextAction}`} <ArrowRight size={14}/>
               </button>
             )}
          </div>
        </div>
      </div>

      <div className="max-w-[1600px] mx-auto p-6 grid grid-cols-12 gap-6">
        {/* Left Col: Info Card */}
        <aside className="col-span-12 lg:col-span-3 space-y-6">
          <div className="bg-[#11151c] border border-slate-800 rounded-xl p-5 shadow-sm">
            <h3 className="text-[10px] font-bold text-slate-500 uppercase tracking-widest mb-6">Incident Details</h3>
            <div className="space-y-6">
                <DetailRow icon={<User size={16}/>} label="Commander" value={data.assignedTo || "Unassigned"} />
                <DetailRow icon={<MapPin size={16}/>} label="Location" value={data.location} />
                <DetailRow icon={<Clock size={16}/>} label="Started" value={new Date(data.createdAt).toLocaleString()} />
                <DetailRow icon={<Shield size={16}/>} label="Type" value={data.type} />
            </div>
          </div>
        </aside>

        {/* Center Col: Timeline & Activity */}
        <main className="col-span-12 lg:col-span-6 space-y-6">
          {error && (
            <div className="bg-red-900/20 border border-red-800 text-red-400 p-4 rounded-lg flex items-center gap-3 animate-in fade-in slide-in-from-top-2">
              <AlertCircle size={18} /> <span className="text-sm font-medium">{error}</span>
            </div>
          )}

          <div className="relative pl-8 border-l border-slate-800 space-y-8 ml-4">
            {data.history.length > 0 ? data.history.map((item: any, i: number) => (
              <div key={i} className="relative group">
                <div className="absolute -left-[41px] top-1 w-4 h-4 rounded-full bg-blue-600 border-4 border-[#0b0e14] group-hover:scale-125 transition-transform" />
                <div className="flex items-center gap-2 text-xs text-slate-500 mb-2">
                  <span className="font-bold text-slate-300">System</span>
                  <ChevronRight size={12}/>
                  <span>{new Date(item.changedAt).toLocaleTimeString()}</span>
                </div>
                <div className="bg-[#11151c] border border-slate-800 p-4 rounded-xl shadow-sm">
                  <p className="text-sm text-white font-medium">Status Transition</p>
                  <p className="text-sm text-slate-400 mt-1">Moved from <span className="text-slate-200">{item.oldStatus}</span> to <span className="text-blue-400 font-bold">{item.newStatus}</span></p>
                  {item.transitionNotes && <p className="mt-3 text-xs text-slate-500 italic border-t border-slate-800/50 pt-2">{item.transitionNotes}</p>}
                </div>
              </div>
            )) : (
              <div className="text-center py-10 text-slate-600 text-sm italic">No status history records found.</div>
            )}
          </div>
        </main>

        {/* Right Col: Evidence & Resources */}
        <aside className="col-span-12 lg:col-span-3 space-y-6">
          <div className="bg-[#11151c] border border-slate-800 rounded-xl p-5">
            <h3 className="text-[10px] font-bold text-slate-500 uppercase tracking-widest mb-4">Evidence Files</h3>
            <div className="space-y-3">
              {data.evidenceFiles.map((f: string, i: number) => (
                <div key={i} className="p-3 bg-[#0b0e14] border border-slate-800 rounded-lg flex items-center gap-3 group cursor-pointer hover:border-blue-500/50 transition-colors">
                  <div className="p-2 bg-blue-900/20 text-blue-500 rounded-md">📎</div>
                  <span className="text-xs truncate text-slate-400 group-hover:text-slate-200">{f}</span>
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
      <div className="text-slate-600 mt-0.5">{icon}</div>
      <div>
        <p className="text-[9px] font-black text-slate-600 uppercase leading-none mb-1 tracking-tighter">{label}</p>
        <p className="text-sm text-slate-200 font-medium">{value}</p>
      </div>
    </div>
  );
}