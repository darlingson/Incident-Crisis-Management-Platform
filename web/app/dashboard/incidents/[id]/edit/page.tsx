'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { Save, X, Loader2, Edit3, User } from 'lucide-react';

interface AssignableUser {
    id: string;
    fullName: string;
    email: string;
}

export default function EditReportPage() {
    const { id } = useParams<{ id: string }>();
    const router = useRouter();
    
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [users, setUsers] = useState<AssignableUser[]>([]);
    
    const [formData, setFormData] = useState({
        title: '',
        narrative: '',
        description: '',
        impact: '',
        location: '',
        assignedTo: '',
        type: ''
    });

    useEffect(() => {
        const loadPageData = async () => {
            try {
                const [reportRes, usersRes] = await Promise.all([
                    fetch(`/api/reports/${id}`),
                    fetch('/api/reports/assignable-users')
                ]);

                const reportData = await reportRes.json();
                const usersData = await usersRes.json();

                setUsers(usersData);
                setFormData({
                    title: reportData.title || '',
                    narrative: reportData.narrative || '',
                    description: reportData.description || reportData.narrative || '',
                    impact: reportData.impact || 'Low',
                    location: reportData.location || '',
                    assignedTo: reportData.assignedTo || '',
                    type: reportData.type || 'General'
                });
            } catch (error) {
                console.error("Failed to load edit data", error);
            } finally {
                setLoading(false);
            }
        };

        if (id) loadPageData();
    }, [id]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setSaving(true);
        
        try {
            const res = await fetch(`/api/reports/${id}/edit`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData),
            });

            if (res.ok) {
                router.push(`/dashboard/incidents/${id}`);
                router.refresh(); // Ensure details page gets fresh data
            } else {
                const err = await res.json();
                alert(err.error || "Failed to update report");
                setSaving(false);
            }
        } catch (error) {
            setSaving(false);
        }
    };

    if (loading) {
        return (
            <div className="bg-[#0b0e14] min-h-screen flex items-center justify-center text-white">
                <Loader2 className="animate-spin mr-2" /> Loading Incident Details...
            </div>
        );
    }

    return (
        <div className="bg-[#0b0e14] min-h-screen text-slate-300 p-8">
            <div className="max-w-3xl mx-auto bg-[#11151c] border border-slate-800 rounded-xl p-8 shadow-2xl">
                <div className="mb-8">
                    <h1 className="text-2xl font-bold text-white flex items-center gap-2">
                        <Edit3 size={24} className="text-blue-500" /> Edit Incident INC-{id}
                    </h1>
                    <p className="text-slate-500 text-sm mt-1">Modify the core details and ownership of this report.</p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
                    {/* Title */}
                    <div>
                        <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Title</label>
                        <input 
                            required
                            className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none transition-all"
                            value={formData.title}
                            onChange={e => setFormData({...formData, title: e.target.value})}
                        />
                    </div>

                    {/* Narrative */}
                    <div>
                        <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Narrative / Description</label>
                        <textarea 
                            rows={4}
                            required
                            className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none transition-all"
                            value={formData.narrative}
                            onChange={e => setFormData({...formData, narrative: e.target.value, description: e.target.value})}
                        />
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        {/* Impact Select */}
                        <div>
                            <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Impact Level</label>
                            <select 
                                className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none appearance-none cursor-pointer"
                                value={formData.impact}
                                onChange={e => setFormData({...formData, impact: e.target.value})}
                            >
                                <option value="Low">Low</option>
                                <option value="Medium">Medium</option>
                                <option value="High">High</option>
                                <option value="Critical">Critical</option>
                            </select>
                        </div>

                        {/* Assigned To Dropdown */}
                        <div>
                            <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Assigned Commander</label>
                            <div className="relative">
                                <select 
                                    className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none appearance-none cursor-pointer"
                                    value={formData.assignedTo}
                                    onChange={e => setFormData({...formData, assignedTo: e.target.value})}
                                >
                                    <option value="">Unassigned</option>
                                    {users.map(user => (
                                        <option key={user.id} value={user.fullName}>
                                            {user.fullName} ({user.email})
                                        </option>
                                    ))}
                                </select>
                                <div className="absolute inset-y-0 right-3 flex items-center pointer-events-none text-slate-500">
                                    <User size={16} />
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Location & Type */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div>
                            <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Location</label>
                            <input 
                                className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none"
                                value={formData.location}
                                onChange={e => setFormData({...formData, location: e.target.value})}
                            />
                        </div>
                        <div>
                            <label className="block text-xs font-bold text-slate-500 uppercase mb-2">Incident Type</label>
                            <input 
                                className="w-full bg-[#0b0e14] border border-slate-700 rounded-lg p-3 text-white focus:border-blue-500 outline-none"
                                value={formData.type}
                                onChange={e => setFormData({...formData, type: e.target.value})}
                            />
                        </div>
                    </div>

                    {/* Form Actions */}
                    <div className="flex gap-4 pt-6 border-t border-slate-800">
                        <button 
                            type="submit" 
                            disabled={saving}
                            className="flex-1 bg-blue-600 hover:bg-blue-500 text-white font-bold py-3 rounded-lg flex items-center justify-center gap-2 disabled:bg-slate-700 transition-all shadow-lg shadow-blue-900/20"
                        >
                            {saving ? <Loader2 className="animate-spin" /> : <Save size={18} />} 
                            {saving ? 'Saving Changes...' : 'Update Incident'}
                        </button>
                        <button 
                            type="button"
                            onClick={() => router.back()}
                            className="px-8 bg-transparent border border-slate-700 text-slate-400 font-bold py-3 rounded-lg flex items-center gap-2 hover:bg-slate-800 hover:text-white transition-all"
                        >
                            <X size={18} /> Cancel
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}