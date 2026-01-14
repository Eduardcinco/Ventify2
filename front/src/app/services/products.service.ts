import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Product {
  id: number;
  nombre: string;
  precioVenta: number;
  stockActual: number;
  stockMinimo: number;
}

@Injectable({ providedIn: 'root' })
export class ProductsService {
  private base = `${environment.apiUrl}/productos`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.base, { withCredentials: true });
  }

  create(payload: { nombre: string; descripcion?: string; precioVenta: number; precioCompra: number; cantidadInicial: number; stockMinimo: number; }): Observable<any> {
    return this.http.post(this.base, {
      Nombre: payload.nombre,
      Descripcion: payload.descripcion,
      PrecioVenta: payload.precioVenta,
      PrecioCompra: payload.precioCompra,
      CantidadInicial: payload.cantidadInicial,
      StockMinimo: payload.stockMinimo
    }, { withCredentials: true });
  }
}
