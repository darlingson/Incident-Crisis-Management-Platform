'use client';

import { useEffect, useState } from 'react';
import { useParams } from 'next/navigation';

export default function Page() {
  const { id } = useParams<{ id: string }>();
  const [data, setData] = useState<any>(null);

  useEffect(() => {
    if (!id) return;
    fetch(`/api/reports/${id}`)
      .then((r) => r.json())
      .then(setData)
      .catch(console.error);
  }, [id]);

  return (
    <span>
      The id is {id} and the data is {JSON.stringify(data)}
    </span>
  );
}