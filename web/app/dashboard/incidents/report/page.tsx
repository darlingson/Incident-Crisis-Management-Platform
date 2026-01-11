"use client";

import React, { useState } from "react";
import { 
  Shield, Lock, Users, Building, Globe, 
  AlertCircle, Upload, Trash2, Send, Save 
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from "@/components/ui/select";

export default function IncidentReportForm() {
  const [loading, setLoading] = useState(false);
  const [incidentType, setIncidentType] = useState("Safety");

  const types = [
    { id: "Safety", icon: Shield },
    { id: "Security", icon: Lock },
    { id: "HR Policy", icon: Users },
    { id: "Facilities", icon: Building },
    { id: "Reputational", icon: Globe },
  ];

  return (
    <div className="max-w-4xl mx-auto py-10 px-6 space-y-10 animate-in fade-in slide-in-from-bottom-4 duration-700">
      {/* Header */}
      <div className="space-y-2">
        <h1 className="text-4xl font-bold tracking-tight">Report an Incident</h1>
        <p className="text-zinc-500 text-lg">
          Please provide details below. Your report helps us maintain a safe environment for everyone.
        </p>
      </div>

      {/* Emergency Alert */}
      <div className="bg-red-500/10 border border-red-500/20 rounded-xl p-4 flex items-center gap-4">
        <div className="bg-red-500/20 p-2 rounded-full">
          <AlertCircle className="w-5 h-5 text-red-500" />
        </div>
        <div className="text-sm">
          <p className="font-bold text-red-200">Is this a life-threatening emergency?</p>
          <p className="text-red-400/80">If you or someone else is in immediate danger, please stop and call 911 immediately.</p>
        </div>
      </div>

      {/* Anonymous Mode */}
      <div className="flex items-center justify-between p-4 bg-zinc-900/40 border border-zinc-800 rounded-xl">
        <div className="flex gap-4 items-center">
          <div className="p-2 bg-zinc-800 rounded-lg">
            <Lock className="w-5 h-5 text-zinc-400" />
          </div>
          <div>
            <p className="font-semibold text-zinc-200">Anonymous Mode</p>
            <p className="text-xs text-zinc-500">Submit this report without linking it to your employee profile.</p>
          </div>
        </div>
        <Switch />
      </div>

      <form className="space-y-12">
        {/* Step 1: Incident Type */}
        <div className="space-y-6">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-full bg-blue-600 flex items-center justify-center text-sm font-bold">1</div>
            <h3 className="text-xl font-bold">Incident Type</h3>
          </div>
          <div className="grid grid-cols-2 md:grid-cols-5 gap-3">
            {types.map((t) => (
              <button
                key={t.id}
                type="button"
                onClick={() => setIncidentType(t.id)}
                className={`flex flex-col items-center justify-center p-6 rounded-xl border transition-all gap-3 ${
                  incidentType === t.id 
                  ? "bg-blue-600/10 border-blue-500 text-blue-500 ring-1 ring-blue-500" 
                  : "bg-zinc-900/40 border-zinc-800 text-zinc-500 hover:bg-zinc-800"
                }`}
              >
                <t.icon className="w-6 h-6" />
                <span className="text-xs font-semibold">{t.id}</span>
              </button>
            ))}
          </div>
        </div>

        {/* Step 2: Incident Details */}
        <div className="space-y-6">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-full bg-blue-600 flex items-center justify-center text-sm font-bold">2</div>
            <h3 className="text-xl font-bold">Incident Details</h3>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-2">
              <Label className="text-zinc-400">Short Description (Title)</Label>
              <Input placeholder="e.g., Slip and fall in lobby" className="bg-zinc-900/50 border-zinc-800 h-12" />
            </div>
            <div className="space-y-2">
              <Label className="text-zinc-400">Impact Level</Label>
              <Select>
                <SelectTrigger className="bg-zinc-900/50 border-zinc-800 h-12">
                  <SelectValue placeholder="Select impact level..." />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="low">Low - Minimal Distruption</SelectItem>
                  <SelectItem value="medium">Medium - Operational Impact</SelectItem>
                  <SelectItem value="high">High - Safety Concern</SelectItem>
                  <SelectItem value="critical">Critical - Immediate Action</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-2">
            <Label className="text-zinc-400">Location</Label>
            <Select>
              <SelectTrigger className="bg-zinc-900/50 border-zinc-800 h-12">
                <SelectValue placeholder="Select location..." />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="hq">Headquarters (Main Lobby)</SelectItem>
                <SelectItem value="warehouse">East Warehouse</SelectItem>
                <SelectItem value="data">Data Center</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label className="text-zinc-400">Detailed Narrative</Label>
            <Textarea 
              placeholder="Please describe exactly what happened. Include names of people involved if known." 
              className="bg-zinc-900/50 border-zinc-800 min-h-[150px] resize-none"
            />
          </div>
        </div>

        {/* Step 3: Evidence */}
        <div className="space-y-6">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-full bg-blue-600 flex items-center justify-center text-sm font-bold">3</div>
            <h3 className="text-xl font-bold">Evidence & Attachments</h3>
          </div>

          <div className="border-2 border-dashed border-zinc-800 rounded-xl p-12 flex flex-col items-center justify-center bg-zinc-900/20 hover:bg-zinc-900/40 transition-colors cursor-pointer group">
            <div className="w-12 h-12 rounded-full bg-zinc-800 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
              <Upload className="w-6 h-6 text-zinc-400" />
            </div>
            <p className="text-sm font-medium"><span className="text-blue-500">Click to upload</span> or drag and drop</p>
            <p className="text-xs text-zinc-500 mt-1 uppercase tracking-wider">SVG, PNG, JPG or PDF (MAX. 10MB)</p>
          </div>

          {/* File Preview Card */}
          <div className="bg-zinc-900/40 border border-zinc-800 rounded-xl p-4 flex items-center justify-between">
            <div className="flex items-center gap-4">
              <div className="bg-zinc-800 p-2 rounded-lg">
                <Upload className="w-4 h-4 text-zinc-500" />
              </div>
              <div>
                <p className="text-sm font-medium">evidence_photo_01.jpg</p>
                <p className="text-xs text-zinc-500">2.4 MB</p>
              </div>
            </div>
            <Button variant="ghost" size="icon" className="text-zinc-500 hover:text-red-500">
              <Trash2 className="w-4 h-4" />
            </Button>
          </div>
        </div>

        {/* Actions */}
        <div className="flex items-center justify-end gap-4 pt-6 border-t border-zinc-800">
          <Button variant="ghost" className="px-8 h-12 font-bold text-zinc-400 hover:text-white">
            <Save className="w-4 h-4 mr-2" />
            Save as Draft
          </Button>
          <Button className="bg-blue-600 hover:bg-blue-500 px-8 h-12 font-bold rounded-lg shadow-lg shadow-blue-500/20">
            Submit Report
            <Send className="w-4 h-4 ml-2" />
          </Button>
        </div>
      </form>
    </div>
  );
}