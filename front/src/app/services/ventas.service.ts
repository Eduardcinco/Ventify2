import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class VentasService {
  private base = `${environment.apiUrl}/ventas`;
  constructor(private http: HttpClient) {}

  create(payload: { items: { productoId: number; cantidad: number; precio: number; }[]; total: number; paymentMethod?: string; montoRecibido?: number; cambio?: number; }): Observable<any> {
    return this.http.post(this.base, {
      Items: payload.items.map(i => ({ ProductoId: i.productoId, Cantidad: i.cantidad, Precio: i.precio })),
      Total: payload.total,
      PaymentMethod: payload.paymentMethod,
      MontoRecibido: payload.montoRecibido,
      Cambio: payload.cambio
    }, { withCredentials: true });
  }

  list(): Observable<any[]> {
    return this.http.get<any[]>(this.base, { withCredentials: true });
  }
}
