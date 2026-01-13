'use client';

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';
import { 
  Clock, MapPin, AlertCircle, User, 
  MessageSquare, Shield, CheckCircle, ArrowRight 
} from 'lucide-react';

export default function IncidentDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!id) return;
    fetch(`/api/reports/${id}`)
      .then((r) => r.json())
      .then((json) => {
        setData(json);
        setLoading(false);
      })
      .catch(console.error);
  }, [id]);

  if (loading) return <div className="bg-[#0b0e14] min-h-screen text-white p-8">Loading...</div>;
  if (!data) return <div className="bg-[#0b0e14] min-h-screen text-white p-8">Incident not found.</div>;

  return (
    <div className="bg-[#0b0e14] min-h-screen text-slate-300 font-sans p-6">
      {/* Header Section */}
      <header className="mb-8">
        <div className="flex items-center gap-3 text-xs font-bold mb-2">
          <span className="bg-red-900/40 text-red-500 px-2 py-0.5 rounded border border-red-800/50 uppercase">
            SEV-1 {data.impact}
          </span>
          <span className="bg-blue-900/40 text-blue-400 px-2 py-0.5 rounded border border-blue-800/50 uppercase">
            {data.status}
          </span>
          <span className="text-slate-500">INC-{data.id}</span>
        </div>
        <h1 className="text-3xl font-bold text-white mb-2">{data.title}</h1>
        <p className="text-slate-400">{data.narrative}</p>
      </header>

      {/* Progress Tracker mimicking the image */}
      <div className="grid grid-cols-4 gap-1 mb-8">
        {['Triage', 'Investigating', 'Mitigated', 'Resolved'].map((step, i) => (
          <div key={step} className="space-y-2">
            <div className={`h-1.5 rounded-full ${i === 0 ? 'bg-blue-500 shadow-[0_0_8px_rgba(59,130,246,0.5)]' : 'bg-slate-800'}`} />
            <span className={`text-xs ${i === 0 ? 'text-blue-400 font-bold' : 'text-slate-500'}`}>{step}</span>
          </div>
        ))}
      </div>

      <div className="grid grid-cols-12 gap-6">
        {/* Left Sidebar: Details */}
        <aside className="col-span-12 lg:col-span-3 space-y-6">
          <section className="bg-[#11151c] border border-slate-800 rounded-lg p-5">
            <h3 className="text-xs font-bold text-slate-500 uppercase tracking-wider mb-4">Incident Details</h3>
            <div className="space-y-4">
              <DetailItem icon={<User size={16}/>} label="Assigned To" value={data.assignedTo || 'Unassigned'} />
              <DetailItem icon={<MapPin size={16}/>} label="Location" value={data.location} />
              <DetailItem icon={<Clock size={16}/>} label="Impact Start" value={new Date(data.createdAt).toLocaleString()} />
              <DetailItem icon={<Shield size={16}/>} label="Type" value={data.type} />
            </div>
          </section>

          <section className="bg-[#11151c] border border-slate-800 rounded-lg p-5">
            <h3 className="text-xs font-bold text-slate-500 uppercase tracking-wider mb-4">Categories</h3>
            <div className="flex flex-wrap gap-2">
              {data.categories.map((cat: string) => (
                <span key={cat} className="text-[10px] bg-slate-800 text-slate-300 px-2 py-1 rounded border border-slate-700 uppercase">
                  {cat}
                </span>
              ))}
            </div>
          </section>
        </aside>

        {/* Main Content: Timeline */}
        <main className="col-span-12 lg:col-span-6 space-y-6">
          <div className="bg-[#11151c] border border-slate-800 rounded-lg p-1 flex gap-4 mb-4 text-xs font-bold">
            <button className="px-4 py-2 bg-[#1a1f29] text-white rounded shadow-sm">Public Note</button>
            <button className="px-4 py-2 text-slate-500 hover:text-slate-300">Internal Note</button>
          </div>

          <div className="relative pl-8 space-y-8 before:content-[''] before:absolute before:left-[11px] before:top-2 before:bottom-0 before:w-px before:bg-slate-800">
             {/* Dynamic History Timeline */}
             {data.history.length > 0 ? data.history.map((h: any, i: number) => (
                <TimelineItem 
                  key={i}
                  user={`User ${h.changedBy}`}
                  time={new Date(h.changedAt).toLocaleTimeString()}
                  content={`Status changed from ${h.oldStatus} to ${h.newStatus}.`}
                  notes={h.transitionNotes}
                  isSystem={true}
                />
             )) : (
                <TimelineItem 
                  user="System"
                  time={new Date(data.createdAt).toLocaleTimeString()}
                  content="Incident reported and triage started."
                  isSystem={true}
                />
             )}
          </div>
        </main>

        {/* Right Sidebar: Controls & Audit */}
        <aside className="col-span-12 lg:col-span-3 space-y-6">
          <section className="bg-[#11151c] border border-slate-800 rounded-lg p-5">
            <h3 className="text-xs font-bold text-slate-500 uppercase tracking-wider mb-4">Manage Incident</h3>
            <button className="w-full bg-blue-600 hover:bg-blue-500 text-white font-bold py-2 rounded flex items-center justify-center gap-2 mb-3 transition-colors">
              Move to Mitigated <ArrowRight size={16} />
            </button>
            <button className="w-full bg-[#1a1f29] hover:bg-[#252b37] text-slate-300 font-bold py-2 rounded border border-slate-700 transition-colors">
              Edit Details
            </button>
          </section>

          <section className="bg-[#11151c] border border-slate-800 rounded-lg p-5">
            <h3 className="text-xs font-bold text-slate-500 uppercase tracking-wider mb-4">Evidence</h3>
            <div className="space-y-2">
              {data.evidenceFiles.map((file: string) => (
                <div key={file} className="flex items-center gap-3 bg-[#0b0e14] p-2 rounded border border-slate-800 text-xs">
                  <div className="p-2 bg-blue-900/20 text-blue-400 rounded">📎</div>
                  <span className="truncate text-slate-400">{file}</span>
                </div>
              ))}
            </div>
          </section>
        </aside>
      </div>
    </div>
  );
}

/* Helper Components for clean code */
function DetailItem({ icon, label, value }: { icon: any, label: string, value: string }) {
  return (
    <div className="flex items-start gap-3">
      <div className="text-slate-500 mt-1">{icon}</div>
      <div>
        <p className="text-[10px] font-bold text-slate-600 uppercase tracking-tighter">{label}</p>
        <p className="text-sm text-slate-300 font-medium">{value}</p>
      </div>
    </div>
  );
}

function TimelineItem({ user, time, content, notes, isSystem }: { user: string, time: string, content: string, notes?: string, isSystem?: boolean }) {
  return (
    <div className="relative">
      <div className={`absolute -left-[28px] top-0 w-[18px] h-[18px] rounded-full border-2 border-[#0b0e14] ${isSystem ? 'bg-blue-600' : 'bg-slate-700'}`} />
      <div className="text-xs mb-1 flex items-center gap-2">
        <span className="font-bold text-slate-200">{user}</span>
        <span className="text-slate-500">{time}</span>
      </div>
      <div className="bg-[#161b24] border border-slate-800 rounded-lg p-4 text-sm">
        <p className="text-slate-300">{content}</p>
        {notes && <p className="mt-2 text-slate-500 italic border-l-2 border-slate-700 pl-3">{notes}</p>}
      </div>
    </div>
  );
}