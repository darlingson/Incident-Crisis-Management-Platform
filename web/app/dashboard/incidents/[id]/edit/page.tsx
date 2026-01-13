'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { Save, X, Loader2,Edit3 } from 'lucide-react';

export default function EditReportPage() {
    const { id } = useParams<{ id: string }>();
    const router = useRouter();
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [formData, setFormData] = useState({
        title: '',
        narrative: '',
        impact: '',
        location: '',
        assignedTo: ''
    });

    useEffect(() => {
        fetch(`/api/reports/${id}`)
            .then(res => res.json())
            .then(data => {
                setFormData({
                    title: data.title,
                    narrative: data.narrative,
                    impact: data.impact,
                    location: data.location,
                    assignedTo: data.assignedTo
                });
                setLoading(false);
            });
    }, [id]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        const res = await fetch(`/api/reports/${id}/edit`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(formData),
        });

        if (res.ok) router.push(`/reports/${id}`);
        else setSaving(false);
    };

    if (loading) return <div className="bg-[#0b0e14] min-h-screen text-white p-10 text-center">Loading Report...</div>;

    return (
        <div className="bg-[#0b0e14] min-h-screen text-slate-300 p-8">
            <div className="max-w-2xl mx-auto bg-[#11151c] border border-slate-800 rounded-xl p-8 shadow-2xl">
                <h1 className="text-2xl font-bold text-white mb-6 flex items-center gap-2">
                    <Edit3 size={24} className="text-blue-500" /> Edit Incident INC-{id}
                </h1>

                <form onSubmit={handleSubmit} className="space-y-6">
                    <div>
                        <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Title</label>
                        <input 
                            className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none transition-all"
                            value={formData.title}
                            onChange={e => setFormData({...formData, title: e.target.value})}
                        />
                    </div>

                    <div>
                        <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Narrative / Description</label>
                        <textarea 
                            rows={4}
                            className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none transition-all"
                            value={formData.narrative}
                            onChange={e => setFormData({...formData, narrative: e.target.value})}
                        />
                    </div>

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Impact Level</label>
                            <select 
                                className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none appearance-none"
                                value={formData.impact}
                                onChange={e => setFormData({...formData, impact: e.target.value})}
                            >
                                <option value="Low">Low</option>
                                <option value="Medium">Medium</option>
                                <option value="High">High</option>
                                <option value="Critical">Critical</option>
                            </select>
                        </div>
                        <div>
                            <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Assigned To</label>
                            <input 
                                className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none"
                                value={formData.assignedTo}
                                onChange={e => setFormData({...formData, assignedTo: e.target.value})}
                            />
                        </div>
                    </div>

                    <div className="flex gap-4 pt-4">
                        <button 
                            type="submit" 
                            disabled={saving}
                            className="flex-1 bg-blue-600 hover:bg-blue-500 text-white font-bold py-3 rounded-lg flex items-center justify-center gap-2 disabled:bg-slate-700 transition-colors"
                        >
                            {saving ? <Loader2 className="animate-spin" /> : <Save size={18} />} Save Changes
                        </button>
                        <button 
                            type="button"
                            onClick={() => router.back()}
                            className="px-6 bg-[#1a1f29] border border-slate-700 text-slate-300 font-bold py-3 rounded-lg flex items-center gap-2 hover:bg-slate-800 transition-colors"
                        >
                            <X size={18} /> Cancel
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}